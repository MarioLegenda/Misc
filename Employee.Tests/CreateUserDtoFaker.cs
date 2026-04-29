using Bogus;
using Bogus.Extensions;
using Employees.Auth;
using Employees.IncomingDTO;

namespace Employee.Tests;

public class CreateUserDtoFaker : Faker<RegisterDto>
{
    public CreateUserDtoFaker()
    {
        RuleFor(e => e.Name, f => f.Name.FirstName());
        RuleFor(e => e.LastName, f => f.Name.LastName());
        RuleFor(e => e.Email, f => f.Person.Email);
        RuleFor(e => e.Password, f => f.Person.UserName);
    }
}