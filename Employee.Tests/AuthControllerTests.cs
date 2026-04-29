using Employees.Auth;
using Employees.Models;

namespace Employee.Tests;

using System.Net.Http.Json;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task Register_ReturnsNewUser()
    {
        var (user, dto) = await this.CreateUser();

        Assert.Equal(dto.Name, user.Name);
        Assert.Equal(dto.LastName, user.LastName);
        Assert.Equal(dto.Email, user.Email);
    }
    
    [Fact]
    public async Task Login_ReturnsToken()
    {
        var (user, dto) = await this.CreateUser();
        
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = user.Email,
            Password = dto.Password,
        });
        
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    private async Task<(User, RegisterDto)> CreateUser()
    {
        var faker = new CreateUserDtoFaker();

        var dto = faker.Generate();

        var response = await _client.PostAsJsonAsync("/api/auth/register", dto);
        
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        
        var user = await response.Content.ReadFromJsonAsync<User>();

        Assert.NotNull(user);

        return (user, dto);
    }
}