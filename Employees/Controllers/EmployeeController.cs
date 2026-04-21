namespace Employees.Controllers;

using Employees.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    [Route("{id}")]
    [HttpGet]
    public async  Task<IActionResult> Index(EmployeesContext ctx, int id)
    {
        var emp = await ctx.Employees
            .Where(e => e.Id == id)
            .Select(e => new EmployeeDto()
            {
                Id = e.Id,
                BirthDate = e.BirthDate,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Gender = e.Gender,
                HireDate = e.HireDate,

                DepartmentEmployees = e.DepartmentEmployees
                    .Select(de => new DepartmentEmployeeDto
                    {
                        DepartmentId = de.DepartmentId,
                        DepartmentName = de.Department.DeptName,
                    })
                    .ToList(),
                
                Titles = e.Titles
                    .Select(de => new TitleDTO()
                    {
                        Title1 = de.Title1,
                        FromDate = de.FromDate,
                        ToDate = de.ToDate,
                        
                    })
                    .ToList(),
                
                DepartmentManagers = e.DepartmentManagers
                    .Select(de => new DepartmentManagerDTO()
                    {
                        DepartmentId = de.DepartmentId,
                        Department = new DepartmentDTO()
                        {
                            Id = de.Department.Id,
                            DeptName = de.Department.DeptName,
                        },
                    })
                    .ToList(),

                Salaries = e.Salaries
                    .Select(s => new SalaryDto
                    {
                        Amount = s.Amount,
                        FromDate = s.FromDate,
                        ToDate = s.ToDate,
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (emp is null)
        {
            return NotFound(new
            {
                status = 404,
                message = "Not found",
            });
        }

        return Ok(emp);
    }
}