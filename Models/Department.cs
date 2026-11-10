using System.ComponentModel.DataAnnotations;

namespace HRSystem.Models;

public class Department
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nazwa działu jest wymagana.")]
    [StringLength(80)]
    [Display(Name = "Nazwa działu")]
    public string Name { get; set; } = string.Empty;

    [StringLength(120)]
    [Display(Name = "Lokalizacja")]
    public string? Location { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
