using Employees.IncomingDTO;

namespace Employee.Tests;

using Employees.OutgoingDTO;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Employees.Models;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly HttpClient _client;

    public string Token;
    
    public CustomWebApplicationFactory()
    {
        _client = this.CreateClient();
    }
    
    public async Task InitializeAsync()
    {
        var faker = new CreateUserDtoFaker();

        var dto = faker.Generate();

        var registerRes = await _client.PostAsJsonAsync("/api/auth/register", dto);
        
        Assert.Equal(System.Net.HttpStatusCode.Created, registerRes.StatusCode);
        
        var user = await registerRes.Content.ReadFromJsonAsync<User>();

        Assert.NotNull(user);
        
        var loginRes = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = user.Email,
            Password = dto.Password,
        });
        
        Assert.Equal(System.Net.HttpStatusCode.OK, loginRes.StatusCode);
        
        var token = await loginRes.Content.ReadFromJsonAsync<LoginToken>();

        Assert.NotNull(token);
        Assert.False(string.IsNullOrEmpty(token.Token));

        this.Token = token.Token;
    }

    public async Task DisposeAsync()
    {
    }
}

public class EmployeeControllerTest: IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly string token;

    public EmployeeControllerTest(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        this.token = factory.Token;
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this.token);
    }
    
    [Fact]
    public async Task GetEmployees_ReturnsOk()
    {
        var response = await _client.GetAsync($"/api/employee?page=1&pageSize=20");
        
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        
        var employees = await response.Content.ReadFromJsonAsync<List<EmployeeDto>>();

        Assert.NotNull(employees);
        Assert.True(employees.Count == 20);
        
        foreach (var emp in employees)
        {
            Assert.NotNull(emp);
            
            Assert.False(string.IsNullOrEmpty(emp.FirstName));
            Assert.False(string.IsNullOrEmpty(emp.LastName));

            Assert.NotEqual(default, emp.BirthDate);
            Assert.NotEqual(default, emp.HireDate);
            
            Assert.True(emp.Salaries.Count > 0);
            Assert.True(emp.Titles.Count > 0);
        }
    }

    [Fact]
    public async Task GetEmployeeById_ReturnsOk()
    {
        var (employee, _) = await CreateEmployee();
        
        var response = await _client.GetAsync($"/api/employee/{employee.Id}");
        
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        
        var emp = await response.Content.ReadFromJsonAsync<EmployeeDto>();
        
        Assert.NotNull(emp);
        Assert.Equal(emp.FirstName, employee.FirstName);
        Assert.Equal(emp.LastName, employee.LastName);

        Assert.Equal(emp.BirthDate.Date, employee.BirthDate.Date);
        Assert.Equal(emp.HireDate.Date, employee.HireDate.Date);
        
        Assert.Equal(emp.Salaries.First().FromDate, employee.Salaries.First().FromDate);
        Assert.Equal(emp.Salaries.First().ToDate, employee.Salaries.First().ToDate);
        
        Assert.Equal(emp.Titles.First().Title1, employee.Titles.First().Title1);
        Assert.Equal(emp.Titles.First().FromDate, employee.Titles.First().FromDate);
        Assert.Equal(emp.Titles.First().ToDate, employee.Titles.First().ToDate);
        
        Assert.Equal(emp.DepartmentEmployees.First().DepartmentId, employee.DepartmentEmployees.First().DepartmentId);
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
        
        var employee = await response.Content.ReadFromJsonAsync<Employee>();

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

        var found = false;
        foreach (var salary in employee.Salaries)
        {
            if (salary.Amount == 234434)
            {
                found = true;
            }
        }
        
        Assert.True(found);
        Assert.Equal("Manager", employee.Titles.First().Title1);
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
