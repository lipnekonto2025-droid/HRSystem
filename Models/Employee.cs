using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRSystem.Models;

public class Employee
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Imię jest wymagane.")]
    [StringLength(50)]
    [Display(Name = "Imię")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nazwisko jest wymagane.")]
    [StringLength(50)]
    [Display(Name = "Nazwisko")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Adres e-mail jest wymagany.")]
    [EmailAddress(ErrorMessage = "Niepoprawny adres e-mail.")]
    [StringLength(120)]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Niepoprawny numer telefonu.")]
    [StringLength(30)]
    [Display(Name = "Telefon")]
    public string? Phone { get; set; }

    [Display(Name = "Data zatrudnienia")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime HireDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, 1000000, ErrorMessage = "Podaj wynagrodzenie z zakresu 0,01 – 1 000 000.")]
    [Display(Name = "Wynagrodzenie brutto (zł)")]
    public decimal Salary { get; set; }

    [Display(Name = "Aktywny")]
    public bool IsActive { get; set; } = true;

    // Klucz obcy + właściwość nawigacyjna: dział pracownika.
    [Display(Name = "Dział")]
    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

    // Klucz obcy + właściwość nawigacyjna: stanowisko pracownika.
    [Display(Name = "Stanowisko")]
    public int PositionId { get; set; }
    public Position? Position { get; set; }

    // Wnioski urlopowe tego pracownika.
    public ICollection<LeaveRequest> Leaves { get; set; } = new List<LeaveRequest>();

    // Tylko do odczytu – nie jest kolumną w bazie.
    public string FullName => $"{FirstName} {LastName}";
}
