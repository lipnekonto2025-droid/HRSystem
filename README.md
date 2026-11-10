# HR System – system zarządzania pracownikami

Prosta aplikacja **ASP.NET Core MVC (.NET 8)** z **Entity Framework Core (Code First)**, bazą **SQL Server LocalDB** i zapytaniami **LINQ**. Bez dodatkowych warstw: model, kontekst, kontrolery, widoki.

## Uruchomienie

1. Zainstaluj **.NET 8 SDK** i SQL Server LocalDB (jest w Visual Studio z pakietem ASP.NET).
2. W folderze projektu: `dotnet restore`, potem `dotnet run`.
3. Otwórz adres z konsoli. Baza `HRSystem` tworzy się i wypełnia danymi przykładowymi przy pierwszym uruchomieniu.


Bootstrap jest ładowany z internetu (CDN) w `Views/Shared/_Layout.cshtml`.

## Struktura

| Folder / plik | Co zawiera |
|---|---|
| `Models/` | klasy encji: `Department`, `Position`, `Employee`, `LeaveRequest` (+ enumy `LeaveType`, `LeaveStatus`) |
| `Data/HRContext.cs` | kontekst EF Core, czyli tabele (`DbSet`) |
| `Data/DbInitializer.cs` | dane przykładowe |
| `Controllers/` | logika: Employees, Departments, Positions, Leaves, Statistics, Home |
| `Views/` | widoki Razor |
| `Program.cs` | start aplikacji, rejestracja kontekstu, trasa domyślna |

## Spełnienie wymagań zadania

| Wymaganie | Gdzie |
|---|---|
| ≥ 3 encje z właściwościami nawigacyjnymi | `Models/` (4 encje) |
| Pełny CRUD | `EmployeesController` |
| Wyszukiwanie, sortowanie, stronicowanie | `EmployeesController.Index` |
| Dane powiązane w szczegółach | `Departments/Details` (pracownicy działu), `Employees/Details` (wnioski urlopowe) |
| Statystyki w osobnej zakładce | `StatisticsController` + `Views/Statistics/Index.cshtml` |
| Szybka nawigacja | nazwy pracowników i działów są linkami |
| Funkcja tematu | wnioski urlopowe: składanie, zatwierdzanie, odrzucanie (`LeavesController`) |

## LINQ w projekcie

`Where`, `OrderBy`, `OrderByDescending`, `GroupBy`, `Select`, `Any`, `Sum`, `Average`, `Count`, `Skip`, `Take`, `Include` / `ThenInclude`, `ToDictionary` – głównie w `EmployeesController.Index` i `StatisticsController.Index`.

## Reguły

- Pensja pracownika musi mieścić się w widełkach jego stanowiska.
- Wniosek urlopowy: data końcowa nie wcześniej niż początkowa, brak nakładania się wniosków tego samego pracownika.
