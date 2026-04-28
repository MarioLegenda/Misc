using Employees.OutgoingDTO;

namespace Employee.Tests;

using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
}

public class EmployeeControllerTest: IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public EmployeeControllerTest(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task UpdateEmployee_Returns_Ok()
    {
        var dto = new
        {
            id = 110022,
            firstName = "John",
            lastName = "Doe",
            birthDate = "1990-01-01T00:00:00",
            gender = "M",
            hireDate = "2020-01-01T00:00:00",
            departmentId = "d007",
            salary = 234434,
            title = "Title"
        };

        var response = await _client.PutAsJsonAsync("/api/employee", dto);

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        
        var employee = await response.Content.ReadFromJsonAsync<EmployeeDto>();

        Assert.NotNull(employee);
        Assert.Equal(110022, employee.Id);
        Assert.Equal("John", employee.FirstName);
        Assert.Equal("Doe", employee.LastName);
        
        var expectedBirthDate = DateTime.SpecifyKind(
            DateTime.Parse("1990-01-01T00:00:00"),
            DateTimeKind.Unspecified
        );
        
        var expectedHireDate = DateTime.SpecifyKind(
            DateTime.Parse("2020-01-01T00:00:00"),
            DateTimeKind.Unspecified
        );
        
        Assert.Equal(expectedBirthDate, employee.BirthDate);
        Assert.Equal(expectedHireDate, employee.HireDate);
        Assert.Equal("M", employee.Gender);
        
        Assert.Equal(234434, employee.Salaries[0].Amount);
    }
}
