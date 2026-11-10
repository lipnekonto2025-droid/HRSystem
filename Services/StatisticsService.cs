using HRSystem.Data;
using HRSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Services;

public interface IStatisticsService
{
    Task<StatisticsViewModel> GetStatisticsAsync();
}

public class StatisticsService : IStatisticsService
{
    private readonly HRContext _db;

    public StatisticsService(HRContext db)
    {
        _db = db;
    }

    public async Task<StatisticsViewModel> GetStatisticsAsync()
    {
        var year = DateTime.Today.Year;

        var departments = await _db.Departments.AsNoTracking()
            .Include(d => d.Employees)
            .OrderBy(d => d.Name)
            .ToListAsync();

        var employees = await _db.Employees.AsNoTracking().ToListAsync();

        var vacations = await _db.LeaveRequests.AsNoTracking()
            .Include(l => l.Employee)
            .Where(l => l.Type == LeaveType.Vacation
                     && l.Status == LeaveStatus.Approved
                     && l.StartDate.Year == year)
            .ToListAsync();

        var active = employees.Where(e => e.IsActive).ToList();

        return new StatisticsViewModel
        {
            Year = year,
            EmployeeCount = employees.Count,
            ActiveCount = active.Count,
            TotalPayroll = active.Sum(e => e.Salary),
            AverageSalary = active.Any() ? active.Average(e => e.Salary) : 0,

            Departments = departments.Select(d =>
            {
                var act = d.Employees.Where(e => e.IsActive).ToList();
                return new DepartmentStat
                {
                    Id = d.Id,
                    Name = d.Name,
                    Count = act.Count,
                    AvgSalary = act.Any() ? act.Average(e => e.Salary) : 0,
                    Payroll = act.Sum(e => e.Salary)
                };
            }).OrderByDescending(s => s.Count).ToList(),

            HiresByYear = employees
                .GroupBy(e => e.HireDate.Year)
                .OrderBy(g => g.Key)
                .Select(g => new YearStat { Year = g.Key, Count = g.Count() })
                .ToList(),

            TopVacation = vacations
                .GroupBy(l => l.EmployeeId)
                .Select(g => new VacationStat
                {
                    EmployeeId = g.Key,
                    Name = g.First().Employee!.FullName,
                    Days = g.Sum(l => l.Days)
                })
                .OrderByDescending(v => v.Days)
                .Take(5)
                .ToList()
        };
    }
}
