using System.ComponentModel.DataAnnotations;
using System.Reflection;
using HRSystem.Models;

namespace HRSystem.Helpers;

/// <summary>Metody rozszerzeniowe budujące zapytania LINQ do pracowników.</summary>
public static class EmployeeQueryExtensions
{
    public static IQueryable<Employee> Search(this IQueryable<Employee> query, string? term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return query;

        term = term.Trim();
        return query.Where(e => e.FirstName.Contains(term)
                             || e.LastName.Contains(term)
                             || e.Email.Contains(term));
    }

    public static IQueryable<Employee> SortBy(this IQueryable<Employee> query, string? sortOrder)
    {
        return sortOrder switch
        {
            "name_desc" => query.OrderByDescending(e => e.LastName).ThenByDescending(e => e.FirstName),
            "date" => query.OrderBy(e => e.HireDate),
            "date_desc" => query.OrderByDescending(e => e.HireDate),
            "salary" => query.OrderBy(e => e.Salary),
            "salary_desc" => query.OrderByDescending(e => e.Salary),
            _ => query.OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
        };
    }
}

public static class EnumExtensions
{
    /// <summary>Zwraca polską nazwę z atrybutu [Display] albo nazwę składowej enuma.</summary>
    public static string GetDisplayName(this Enum value)
    {
        var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
        return member?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? value.ToString();
    }

    public static string BadgeClass(this LeaveStatus status) => status switch
    {
        LeaveStatus.Approved => "bg-success",
        LeaveStatus.Rejected => "bg-danger",
        _ => "bg-warning text-dark"
    };
}
