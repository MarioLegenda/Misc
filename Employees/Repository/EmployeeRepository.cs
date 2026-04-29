using Employees.OutgoingDTO;
using Microsoft.EntityFrameworkCore;

namespace Employees.Repository;

public class EmployeeRepository
{
    private EmployeesContext _ctx;
    
    public EmployeeRepository(EmployeesContext ctx)
    {
        this._ctx = ctx;
    }
    
    public async Task<EmployeeDto?> GetEmployee(long id)
    {
        var emp = await this._ctx.Employees
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

        return emp;
    }
    
    public async Task<List<EmployeeDto>> GetEmployees(int page = 1, int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        
        var emp = await this._ctx.Employees
            .OrderBy(e => e.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
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
            .ToListAsync();

        return emp;
    }
}