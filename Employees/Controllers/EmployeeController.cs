using Microsoft.AspNetCore.Authorization;

namespace Employees.Controllers;

using Employees.Repository;
using Employees.IncomingDTO;
using Employees.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    [Route("{id}")]
    [HttpGet]
    [Authorize(Roles = "User")]
    public async  Task<IActionResult> GetById(EmployeeRepository repository, long id)
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

    [HttpPost]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> CreateEmployee(
        [FromBody] CreateEmployeeDTO dto,
        EmployeesContext ctx,
        EmployeeRepository repository)
    {
        var emp = new Employee();
        emp.Gender = dto.Gender;
        emp.FirstName = dto.FirstName;
        emp.LastName = dto.LastName;
        emp.HireDate = dto.HireDate;
        emp.BirthDate = dto.BirthDate;

        var salary = new Salary();
        salary.Amount = dto.Salary;
        salary.FromDate = dto.SalaryFromDate;
        salary.ToDate = dto.SalaryToDate;

        var title = new Title();
        title.Title1 = dto.Title;
        title.FromDate = dto.TitleFromDate;
        title.ToDate = dto.TitleToDate;

        emp.Salaries.Add(salary);
        emp.Titles.Add(title);

        ctx.Employees.Add(emp);

        await ctx.SaveChangesAsync();

        var departmentEmployee = new DepartmentEmployee();
        departmentEmployee.DepartmentId = dto.DepartmentId;
        departmentEmployee.EmployeeId = emp.Id;

        ctx.DepartmentEmployees.Add(departmentEmployee);
        
        await ctx.SaveChangesAsync();
        
        return Ok(await repository.GetEmployee(emp.Id));
    }
    
    [HttpPut]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> UpdateEmployee(
        [FromBody] UpdateEmployeeDto dto,
        EmployeeRepository repository,
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

        return Ok(await repository.GetEmployee(dto.Id));
    }
}