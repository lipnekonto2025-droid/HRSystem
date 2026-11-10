namespace HRSystem.Models;

public class StatisticsViewModel
{
    public int Year { get; set; }
    public int EmployeeCount { get; set; }
    public int ActiveCount { get; set; }
    public decimal TotalPayroll { get; set; }
    public decimal AverageSalary { get; set; }
    public List<DepartmentStat> Departments { get; set; } = new();
    public List<YearStat> HiresByYear { get; set; } = new();
    public List<VacationStat> TopVacation { get; set; } = new();
}

public class DepartmentStat
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal AvgSalary { get; set; }
    public decimal Payroll { get; set; }
}

public class YearStat
{
    public int Year { get; set; }
    public int Count { get; set; }
}

public class VacationStat
{
    public int EmployeeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Days { get; set; }
}
