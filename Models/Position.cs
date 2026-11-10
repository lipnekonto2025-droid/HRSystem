using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRSystem.Models;

public class Position
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nazwa stanowiska jest wymagana.")]
    [StringLength(80)]
    [Display(Name = "Nazwa stanowiska")]
    public string Title { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 1000000, ErrorMessage = "Podaj kwotę z zakresu 0 – 1 000 000.")]
    [Display(Name = "Minimalne wynagrodzenie (zł)")]
    public decimal MinSalary { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 1000000, ErrorMessage = "Podaj kwotę z zakresu 0 – 1 000 000.")]
    [Display(Name = "Maksymalne wynagrodzenie (zł)")]
    public decimal MaxSalary { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
