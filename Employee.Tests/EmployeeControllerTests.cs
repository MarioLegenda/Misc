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
    public async Task UpdateEmployee_Returns_NoContent()
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

        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
    }
}
