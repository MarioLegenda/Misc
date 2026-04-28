using Employees.IncomingDTO;

namespace Employee.Tests;

using Employees.OutgoingDTO;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Employees.Models;

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
    public async Task CreateEmployee_ReturnsOk()
    {
        var (employee, dto) = await CreateEmployee();

        Assert.NotNull(employee);
        Assert.Equal(dto.FirstName, employee.FirstName);
        Assert.Equal(dto.LastName, employee.LastName);

        Assert.Equal(dto.BirthDate.Date, employee.BirthDate.Date);
        Assert.Equal(dto.HireDate.Date, employee.HireDate.Date);
        
        Assert.Equal(dto.SalaryFromDate, employee.Salaries.First().FromDate);
        Assert.Equal(dto.SalaryToDate, employee.Salaries.First().ToDate);
        
        Assert.Equal(dto.Title, employee.Titles.First().Title1);
        Assert.Equal(dto.TitleFromDate, employee.Titles.First().FromDate);
        Assert.Equal(dto.TitleToDate, employee.Titles.First().ToDate);
        
        Assert.Equal(dto.DepartmentId, employee.DepartmentEmployees.First().DepartmentId);
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
        Assert.Equal("Manager", employee.Titles[0].Title1);
    }

    private async Task<(Employee, CreateEmployeeDTO)> CreateEmployee()
    {
        var faker = new CreateEmployeeDtoFaker();

        var dto = faker.Generate();
        
        var response = await _client.PostAsJsonAsync("/api/employee", dto);
        
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        
        var employee = await response.Content.ReadFromJsonAsync<Employee>();

        Assert.NotNull(employee);

        return (employee, dto);
    }
}
