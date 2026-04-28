
using Employees.Repository;

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
    public async  Task<IActionResult> Index(EmployeeRepository repository, long id)
    {
        var emp = await repository.GetEmployee(id);

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