using System.ComponentModel.DataAnnotations;

namespace HRSystem.Models;

public enum LeaveType
{
    [Display(Name = "Urlop wypoczynkowy")] Vacation,
    [Display(Name = "Zwolnienie lekarskie")] Sick,
    [Display(Name = "Urlop okolicznościowy")] Occasional,
    [Display(Name = "Urlop bezpłatny")] Unpaid
}

public enum LeaveStatus
{
    [Display(Name = "Oczekuje")] Pending,
    [Display(Name = "Zatwierdzony")] Approved,
    [Display(Name = "Odrzucony")] Rejected
}

public class LeaveRequest
{
    public int Id { get; set; }

    [Display(Name = "Pracownik")]
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    [Display(Name = "Od")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime StartDate { get; set; }

    [Display(Name = "Do")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime EndDate { get; set; }

    [Display(Name = "Rodzaj")]
    public LeaveType Type { get; set; }

    [Display(Name = "Status")]
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;

    [StringLength(300)]
    [Display(Name = "Uwagi")]
    public string? Comment { get; set; }
}
