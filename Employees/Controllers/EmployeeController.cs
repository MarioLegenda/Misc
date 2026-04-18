namespace Employees.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    [Route("")]
    [HttpGet]
    public IActionResult Index() => Ok(new { data = "json" });
}