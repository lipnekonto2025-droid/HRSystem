using HRSystem.Data;
using HRSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Controllers;

public class HomeController : Controller
{
    private readonly HRContext _db;

    public HomeController(HRContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.EmployeeCount = await _db.Employees.CountAsync(e => e.IsActive);
        ViewBag.DepartmentCount = await _db.Departments.CountAsync();
        ViewBag.PendingLeaves = await _db.LeaveRequests.CountAsync(l => l.Status == LeaveStatus.Pending);

        var recent = await _db.Employees.AsNoTracking()
            .Include(e => e.Department)
            .Include(e => e.Position)
            .OrderByDescending(e => e.HireDate)
            .Take(5)
            .ToListAsync();

        return View(recent);
    }

    public IActionResult Error() => Content("Wystąpił nieoczekiwany błąd.");
}
