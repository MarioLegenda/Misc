using Bogus.Extensions;
using Employees.IncomingDTO;

namespace Employee.Tests;

using Bogus;

public class CreateEmployeeDtoFaker : Faker<CreateEmployeeDTO>
{
    public CreateEmployeeDtoFaker()
    {
        RuleFor(e => e.FirstName, f => f.Name.FirstName());

        RuleFor(e => e.LastName, f => f.Name.LastName());

        RuleFor(e => e.Gender, f => f.PickRandom(new[] { "M", "F" }));

        RuleFor(e => e.BirthDate, f =>
            DateTime.SpecifyKind(
                f.Date.Past(40, DateTime.Now.AddYears(-18)), // age 18–58
                DateTimeKind.Utc
            )
        );

        RuleFor(e => e.HireDate, (f, e) =>
            DateTime.SpecifyKind(
                f.Date.Between(e.BirthDate.AddYears(18), DateTime.UtcNow),
                DateTimeKind.Utc
            )
        );

        RuleFor(e => e.DepartmentId, f =>
            $"d{f.Random.Number(1, 9):000}"
        );

        RuleFor(e => e.Salary, f =>
            f.Random.Int(30000, 150000)
        );

        RuleFor(e => e.Title, f =>
            f.Name.JobTitle().ClampLength(1, 50)
        );

        // Salary dates
        RuleFor(e => e.SalaryFromDate, (f, e) =>
            DateOnly.FromDateTime(e.HireDate)
        );

        RuleFor(e => e.SalaryToDate, (f, e) =>
            e.SalaryFromDate.AddYears(f.Random.Int(1, 5))
        );

        // Title dates
        RuleFor(e => e.TitleFromDate, (f, e) =>
            e.SalaryFromDate
        );

        RuleFor(e => e.TitleToDate, (f, e) =>
            e.TitleFromDate.AddYears(f.Random.Int(1, 5))
        );
        
        var departments = new[] { "d001", "d002", "d003", "d004", "d005", "d006", "d007", "d008", "d009" };

        RuleFor(e => e.DepartmentId, f => f.PickRandom(departments));
    }
}