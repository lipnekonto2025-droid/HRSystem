using HRSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Data;

public class HRContext : DbContext
{
    public HRContext(DbContextOptions<HRContext> options) : base(options)
    {
    }

    // Każdy DbSet to jedna tabela w bazie.
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
}
