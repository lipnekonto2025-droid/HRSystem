using HRSystem.Data;
using HRSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Controllers;

public class DepartmentsController : Controller
{
    private readonly HRContext _db;

    public DepartmentsController(HRContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var departments = await _db.Departments
            .Include(d => d.Employees)
            .OrderBy(d => d.Name)
            .ToListAsync();
        return View(departments);
    }

    // Szczegóły działu: lista jego pracowników (nazwy są linkami do ich szczegółów).
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var department = await _db.Departments
            .Include(d => d.Employees)
                .ThenInclude(e => e.Position)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (department == null) return NotFound();
        return View(department);
    }

    public IActionResult Create()
    {
        return View(new Department());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Location")] Department department)
    {
        if (ModelState.IsValid)
        {
            _db.Add(department);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = department.Id });
        }

        return View(department);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var department = await _db.Departments.FindAsync(id);
        if (department == null) return NotFound();
        return View(department);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Location")] Department department)
    {
        if (id != department.Id) return NotFound();

        if (ModelState.IsValid)
        {
            _db.Update(department);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = department.Id });
        }

        return View(department);
    }
}
