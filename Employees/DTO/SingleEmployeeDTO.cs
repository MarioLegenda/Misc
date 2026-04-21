namespace Employees.DTO;

public class EmployeeDto
{
    public long Id { get; set; }
    public DateTime BirthDate { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public EmployeeGender Gender { get; set; }
    public DateTime HireDate { get; set; }

    public List<DepartmentEmployeeDto> DepartmentEmployees { get; set; }
    public List<SalaryDto> Salaries { get; set; }
    public List<DepartmentManagerDTO> DepartmentManagers { get; set; }
    public List<TitleDTO> Titles { get; set; }
}

public class DepartmentDTO
{
    public string Id { get; set; } = null!;

    public string DeptName { get; set; } = null!;
}

public class TitleDTO
{
    public string Title1 { get; set; } = null!;

    public DateOnly FromDate { get; set; }

    public DateOnly? ToDate { get; set; }
}


public class DepartmentManagerDTO
{
    public string DepartmentId { get; set; } = null!;

    public DateOnly FromDate { get; set; }

    public DateOnly ToDate { get; set; }

    public virtual DepartmentDTO Department { get; set; } = null!;
}

public class DepartmentEmployeeDto
{
    public string DepartmentId { get; set; }
    public string DepartmentName { get; set; }
}

public class SalaryDto
{
    public decimal Amount { get; set; }
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
}