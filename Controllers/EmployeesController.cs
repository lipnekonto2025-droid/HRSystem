using HRSystem.Data;
using HRSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Controllers;

public class EmployeesController : Controller
{
    private readonly HRContext _db;

    public EmployeesController(HRContext db)
    {
        _db = db;
    }

    // Lista: wyszukiwanie, filtr po dziale, sortowanie i stronicowanie.
    public async Task<IActionResult> Index(string? sortOrder, string? searchString, int? departmentId, int page = 1)
    {
        const int pageSize = 8;

        var query = _db.Employees
            .Include(e => e.Department)
            .Include(e => e.Position)
            .AsQueryable();

        // Wyszukiwanie
        if (!string.IsNullOrWhiteSpace(searchString))
        {
            query = query.Where(e => e.FirstName.Contains(searchString)
                                  || e.LastName.Contains(searchString)
                                  || e.Email.Contains(searchString));
        }

        // Filtr po dziale
        if (departmentId.HasValue)
        {
            query = query.Where(e => e.DepartmentId == departmentId.Value);
        }

        // Sortowanie
        switch (sortOrder)
        {
            case "name_desc":
                query = query.OrderByDescending(e => e.LastName);
                break;
            case "date":
                query = query.OrderBy(e => e.HireDate);
                break;
            case "date_desc":
                query = query.OrderByDescending(e => e.HireDate);
                break;
            case "salary":
                query = query.OrderBy(e => e.Salary);
                break;
            case "salary_desc":
                query = query.OrderByDescending(e => e.Salary);
                break;
            default:
                query = query.OrderBy(e => e.LastName);
                break;
        }

        // Stronicowanie
        var total = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
        page = Math.Clamp(page, 1, totalPages);

        var employees = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Dane dla widoku (zapamiętują wybrane kryteria w linkach)
        ViewBag.SortOrder = sortOrder;
        ViewBag.SearchString = searchString;
        ViewBag.DepartmentId = departmentId;
        ViewBag.Page = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.Total = total;
        ViewBag.NameSort = (sortOrder == null || sortOrder == "name") ? "name_desc" : "name";
        ViewBag.DateSort = sortOrder == "date" ? "date_desc" : "date";
        ViewBag.SalarySort = sortOrder == "salary" ? "salary_desc" : "salary";
        ViewBag.Departments = new SelectList(await _db.Departments.OrderBy(d => d.Name).ToListAsync(), "Id", "Name", departmentId);

        return View(employees);
    }

    // Szczegóły: dział, stanowisko i wnioski urlopowe dzięki właściwościom nawigacyjnym.
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var employee = await _db.Employees
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Include(e => e.Leaves)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null) return NotFound();
        return View(employee);
    }

    public async Task<IActionResult> Create()
    {
        await FillLists();
        return View(new Employee { HireDate = DateTime.Today });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("FirstName,LastName,Email,Phone,HireDate,Salary,IsActive,DepartmentId,PositionId")] Employee employee)
    {
        await CheckSalary(employee);

        if (ModelState.IsValid)
        {
            _db.Add(employee);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = employee.Id });
        }

        await FillLists(employee.DepartmentId, employee.PositionId);
        return View(employee);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var employee = await _db.Employees.FindAsync(id);
        if (employee == null) return NotFound();

        await FillLists(employee.DepartmentId, employee.PositionId);
        return View(employee);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("Id,FirstName,LastName,Email,Phone,HireDate,Salary,IsActive,DepartmentId,PositionId")] Employee employee)
    {
        if (id != employee.Id) return NotFound();

        await CheckSalary(employee);

        if (ModelState.IsValid)
        {
            try
            {
                _db.Update(employee);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // ktoś usunął pracownika w międzyczasie
                if (!_db.Employees.Any(e => e.Id == id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Details), new { id = employee.Id });
        }

        await FillLists(employee.DepartmentId, employee.PositionId);
        return View(employee);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var employee = await _db.Employees
            .Include(e => e.Department)
            .Include(e => e.Position)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null) return NotFound();
        return View(employee);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var employee = await _db.Employees.FindAsync(id);
        if (employee != null)
        {
            _db.Employees.Remove(employee);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    // Reguła: wynagrodzenie musi mieścić się w widełkach wybranego stanowiska.
    private async Task CheckSalary(Employee employee)
    {
        var position = await _db.Positions.FindAsync(employee.PositionId);
        if (position != null && (employee.Salary < position.MinSalary || employee.Salary > position.MaxSalary))
        {
            ModelState.AddModelError("Salary",
                $"Dla stanowiska „{position.Title}” wynagrodzenie musi mieścić się w przedziale {position.MinSalary:N2} – {position.MaxSalary:N2} zł.");
        }
    }

    // Listy rozwijane w formularzu: działy i stanowiska.
    private async Task FillLists(object? selectedDepartment = null, object? selectedPosition = null)
    {
        ViewBag.Departments = new SelectList(await _db.Departments.OrderBy(d => d.Name).ToListAsync(), "Id", "Name", selectedDepartment);
        ViewBag.Positions = new SelectList(await _db.Positions.OrderBy(p => p.Title).ToListAsync(), "Id", "Title", selectedPosition);
    }
}
