using HRSystem.Models;

namespace HRSystem.Data;

public static class DbInitializer
{
    public static void Seed(HRContext context)
    {
        if (context.Departments.Any())
            return;

        var departments = new[]
        {
            new Department { Name = "IT", Location = "Warszawa, ul. Złota 10" },
            new Department { Name = "Kadry", Location = "Warszawa, ul. Złota 10" },
            new Department { Name = "Księgowość", Location = "Kraków, ul. Długa 5" },
            new Department { Name = "Sprzedaż", Location = "Gdańsk, ul. Morska 21" },
            new Department { Name = "Logistyka", Location = "Łódź, ul. Piotrkowska 80" }
        };

        var positions = new[]
        {
            new Position { Title = "Programista Junior", MinSalary = 6000, MaxSalary = 9000 },
            new Position { Title = "Programista Senior", MinSalary = 12000, MaxSalary = 20000 },
            new Position { Title = "Specjalista ds. kadr", MinSalary = 5500, MaxSalary = 9000 },
            new Position { Title = "Księgowy", MinSalary = 5500, MaxSalary = 9500 },
            new Position { Title = "Przedstawiciel handlowy", MinSalary = 5000, MaxSalary = 12000 },
            new Position { Title = "Kierownik działu", MinSalary = 12000, MaxSalary = 22000 },
            new Position { Title = "Magazynier", MinSalary = 4500, MaxSalary = 6500 }
        };

        context.Departments.AddRange(departments);
        context.Positions.AddRange(positions);

        // (imię, nazwisko, indeks działu, indeks stanowiska, pensja, data zatrudnienia)
        var data = new (string First, string Last, int Dept, int Pos, decimal Salary, DateTime Hired)[]
        {
            ("Anna", "Kowalska", 0, 5, 16000m, new DateTime(2019, 3, 1)),
            ("Piotr", "Nowak", 0, 1, 15000m, new DateTime(2020, 6, 15)),
            ("Marek", "Wojcik", 0, 1, 13500m, new DateTime(2021, 1, 10)),
            ("Ewa", "Kaminska", 0, 0, 7500m, new DateTime(2023, 9, 1)),
            ("Tomasz", "Lewandowski", 0, 0, 6800m, new DateTime(2024, 2, 12)),
            ("Joanna", "Zielinska", 1, 5, 14000m, new DateTime(2018, 5, 20)),
            ("Magdalena", "Szymanska", 1, 2, 6800m, new DateTime(2022, 3, 7)),
            ("Pawel", "Wozniak", 1, 2, 6200m, new DateTime(2024, 8, 19)),
            ("Krzysztof", "Dabrowski", 2, 5, 15500m, new DateTime(2017, 11, 2)),
            ("Agnieszka", "Kozlowska", 2, 3, 7200m, new DateTime(2021, 4, 26)),
            ("Michal", "Jankowski", 2, 3, 6600m, new DateTime(2023, 1, 16)),
            ("Barbara", "Mazur", 3, 5, 17000m, new DateTime(2016, 9, 12)),
            ("Adam", "Krawczyk", 3, 4, 8500m, new DateTime(2020, 10, 5)),
            ("Natalia", "Piotrowska", 3, 4, 7200m, new DateTime(2022, 7, 18)),
            ("Rafal", "Grabowski", 3, 4, 6100m, new DateTime(2024, 5, 6)),
            ("Monika", "Pawlowska", 3, 4, 9800m, new DateTime(2019, 12, 2)),
            ("Jakub", "Michalski", 4, 5, 13000m, new DateTime(2018, 2, 26)),
            ("Dariusz", "Zajac", 4, 6, 5400m, new DateTime(2021, 9, 13)),
            ("Sylwia", "Krol", 4, 6, 5100m, new DateTime(2023, 3, 20)),
            ("Lukasz", "Wieczorek", 4, 6, 4900m, new DateTime(2024, 11, 4))
        };

        var employees = data.Select((d, i) => new Employee
        {
            FirstName = d.First,
            LastName = d.Last,
            Email = $"{d.First}.{d.Last}@firma.pl".ToLowerInvariant(),
            Phone = $"+48 600 100 {100 + i}",
            HireDate = d.Hired,
            Salary = d.Salary,
            IsActive = true,
            Department = departments[d.Dept],
            Position = positions[d.Pos]
        }).ToList();

        employees[14].IsActive = false; // przykład pracownika nieaktywnego
        context.Employees.AddRange(employees);

        var y = DateTime.Today.Year;
        context.LeaveRequests.AddRange(
            new LeaveRequest { Employee = employees[1], Type = LeaveType.Vacation, StartDate = new DateTime(y, 7, 10), EndDate = new DateTime(y, 7, 21), Status = LeaveStatus.Approved, Comment = "Urlop letni" },
            new LeaveRequest { Employee = employees[3], Type = LeaveType.Vacation, StartDate = new DateTime(y, 8, 3), EndDate = new DateTime(y, 8, 14), Status = LeaveStatus.Approved },
            new LeaveRequest { Employee = employees[6], Type = LeaveType.Sick, StartDate = new DateTime(y, 3, 3), EndDate = new DateTime(y, 3, 5), Status = LeaveStatus.Approved },
            new LeaveRequest { Employee = employees[12], Type = LeaveType.Vacation, StartDate = new DateTime(y, 12, 22), EndDate = new DateTime(y, 12, 31), Status = LeaveStatus.Pending, Comment = "Święta" },
            new LeaveRequest { Employee = employees[17], Type = LeaveType.Vacation, StartDate = new DateTime(y, 11, 23), EndDate = new DateTime(y, 11, 24), Status = LeaveStatus.Pending },
            new LeaveRequest { Employee = employees[0], Type = LeaveType.Vacation, StartDate = new DateTime(y, 5, 4), EndDate = new DateTime(y, 5, 8), Status = LeaveStatus.Rejected, Comment = "Konflikt z wdrożeniem" },
            new LeaveRequest { Employee = employees[9], Type = LeaveType.Vacation, StartDate = new DateTime(y, 6, 1), EndDate = new DateTime(y, 6, 19), Status = LeaveStatus.Approved }
        );

        context.SaveChanges();
    }
}
