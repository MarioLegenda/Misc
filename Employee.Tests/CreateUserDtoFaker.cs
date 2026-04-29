using Bogus;
using Employees.Auth;

namespace Employee.Tests;

public class CreateUserDtoFaker : Faker<RegisterDto>
{
    public CreateUserDtoFaker()
    {
        RuleFor(e => e.Name, f => f.Name.FirstName());
        RuleFor(e => e.LastName, f => f.Name.LastName());
        RuleFor(e => e.Email, f =>
            $"user{Guid.NewGuid().ToString("N")[..8]}@example.com");

        RuleFor(e => e.Password, f =>
            $"P@ssword{Guid.NewGuid().ToString("N")[..8]}");
    }
}