using HRSystem.Data;
using HRSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Controllers;

public class StatisticsController : Controller
{
    private readonly HRContext _db;

    public StatisticsController(HRContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var employees = await _db.Employees.ToListAsync();
        var active = employees.Where(e => e.IsActive).ToList();

        // Liczby ogólne
        ViewBag.Total = employees.Count;
        ViewBag.ActiveCount = active.Count;
        ViewBag.Payroll = active.Sum(e => e.Salary);
        ViewBag.Average = active.Any() ? active.Average(e => e.Salary) : 0m;

        // Zatrudnienia według roku (GroupBy)
        ViewBag.ByYear = employees
            .GroupBy(e => e.HireDate.Year)
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.Count());

        // Wnioski urlopowe według statusu
        ViewBag.Pending = await _db.LeaveRequests.CountAsync(l => l.Status == LeaveStatus.Pending);
        ViewBag.Approved = await _db.LeaveRequests.CountAsync(l => l.Status == LeaveStatus.Approved);
        ViewBag.Rejected = await _db.LeaveRequests.CountAsync(l => l.Status == LeaveStatus.Rejected);

        // Działy z pracownikami – statystyki po działach liczy widok
        var departments = await _db.Departments
            .Include(d => d.Employees)
            .OrderBy(d => d.Name)
            .ToListAsync();

        return View(departments);
    }
}
