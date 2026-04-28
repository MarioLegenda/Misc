
namespace Employees.Controllers;

using Employees.IncomingDTO;
using Employees.OutgoingDTO;
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
    
    [HttpPut]
    public async Task<IActionResult> UpdateEmployee(
        [FromBody] UpdateEmployeeDto dto,
        EmployeesContext ctx)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                Code = "invalid_model"
            });
        }
        
        var employee = await ctx.Employees
            .FirstOrDefaultAsync(e => e.Id == dto.Id);

        if (employee == null)
            return NotFound(new
            {
                Code = "employee_not_found"
            });

        var departmentManager =
            await ctx.DepartmentManagers.FirstOrDefaultAsync(e =>
                e.DepartmentId == dto.DepartmentId && e.EmployeeId == dto.Id);

        if (departmentManager == null)
        {
            employee.DepartmentManagers.Clear();
            employee.DepartmentManagers.Add(new DepartmentManager
            {
                EmployeeId = employee.Id,
                DepartmentId = dto.DepartmentId
            });
        }
        else
        {
            departmentManager.DepartmentId = dto.DepartmentId;
            departmentManager.EmployeeId = dto.Id;

            ctx.Update(departmentManager);
        }

        var department = await ctx.Departments.FirstOrDefaultAsync(e => e.Id == dto.DepartmentId);

        if (department == null)
        {
            return NotFound(new
            {
                Code = "department_not_found"
            });
        }

        var salary = await ctx.Salaries.FirstOrDefaultAsync(e => e.EmployeeId == dto.Id);
        if (salary == null)
        {
            employee.Salaries.Clear();
            employee.Salaries.Add(new Salary()
            {
                EmployeeId = employee.Id,
                Amount = dto.Salary,
            });
        }
        else
        {
            salary.Amount = dto.Salary;
            ctx.Update(salary);
        }
        
        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Gender = dto.Gender;
        employee.BirthDate = DateTime.SpecifyKind(dto.BirthDate, DateTimeKind.Utc);
        employee.HireDate = DateTime.SpecifyKind(dto.HireDate, DateTimeKind.Utc);

        await ctx.SaveChangesAsync();

        return NoContent();
    }
}