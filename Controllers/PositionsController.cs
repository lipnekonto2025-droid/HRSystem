using HRSystem.Data;
using HRSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Controllers;

public class PositionsController : Controller
{
    private readonly HRContext _db;

    public PositionsController(HRContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var positions = await _db.Positions
            .Include(p => p.Employees)
            .OrderBy(p => p.Title)
            .ToListAsync();
        return View(positions);
    }

    public IActionResult Create()
    {
        return View(new Position());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,MinSalary,MaxSalary")] Position position)
    {
        if (position.MaxSalary < position.MinSalary)
        {
            ModelState.AddModelError("MaxSalary", "Maksimum nie może być mniejsze od minimum.");
        }

        if (ModelState.IsValid)
        {
            _db.Add(position);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(position);
    }
}
