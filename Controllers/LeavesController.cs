using HRSystem.Data;
using HRSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Controllers;

public class LeavesController : Controller
{
    private readonly HRContext _db;

    public LeavesController(HRContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(LeaveStatus? status)
    {
        ViewBag.Status = status;

        var query = _db.LeaveRequests.Include(l => l.Employee).AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(l => l.Status == status.Value);
        }

        var leaves = await query.OrderByDescending(l => l.StartDate).ToListAsync();
        return View(leaves);
    }

    public async Task<IActionResult> Create(int? employeeId)
    {
        await FillEmployees(employeeId);
        return View(new LeaveRequest
        {
            EmployeeId = employeeId ?? 0,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("EmployeeId,StartDate,EndDate,Type,Comment")] LeaveRequest request)
    {
        // Reguła 1: data końcowa nie może być wcześniejsza niż początkowa.
        if (request.EndDate < request.StartDate)
        {
            ModelState.AddModelError("EndDate", "Data końcowa nie może być wcześniejsza niż początkowa.");
        }

        // Reguła 2: pracownik nie może mieć dwóch wniosków na ten sam termin.
        var overlaps = await _db.LeaveRequests.AnyAsync(l =>
            l.EmployeeId == request.EmployeeId &&
            l.Status != LeaveStatus.Rejected &&
            l.StartDate <= request.EndDate &&
            l.EndDate >= request.StartDate);

        if (overlaps)
        {
            ModelState.AddModelError(string.Empty, "W tym terminie pracownik ma już wniosek.");
        }

        if (ModelState.IsValid)
        {
            request.Status = LeaveStatus.Pending;
            _db.Add(request);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        await FillEmployees(request.EmployeeId);
        return View(request);
    }

    // Zatwierdzenie lub odrzucenie wniosku (status przychodzi z formularza).
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SetStatus(int id, LeaveStatus status)
    {
        var leave = await _db.LeaveRequests.FindAsync(id);
        if (leave == null) return NotFound();

        leave.Status = status;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { status = LeaveStatus.Pending });
    }

    private async Task FillEmployees(object? selected = null)
    {
        var employees = await _db.Employees
            .Where(e => e.IsActive)
            .OrderBy(e => e.LastName)
            .Select(e => new { e.Id, Name = e.LastName + " " + e.FirstName })
            .ToListAsync();

        ViewBag.Employees = new SelectList(employees, "Id", "Name", selected);
    }
}
