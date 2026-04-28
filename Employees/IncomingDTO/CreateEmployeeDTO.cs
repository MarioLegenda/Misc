using System.ComponentModel.DataAnnotations;

namespace Employees.IncomingDTO;

public class CreateEmployeeDTO
{
    [Required]
    public string FirstName { get; set; } = null!;
    
    [Required]
    public string LastName { get; set; } = null!;
    
    [Required]
    public DateTime BirthDate { get; set; }
    
    [Required]
    public string Gender { get; set; }
    
    [Required]
    public DateTime HireDate { get; set; }
    
    [Required]
    public string DepartmentId { get; set; }
    
    [Required]
    public int Salary { get; set; }
    
    [MaxLength(50)]
    [Required]
    public string Title { get; set; }
    
    [Required]
    public DateOnly SalaryFromDate { get; set; }
    
    [Required]
    public DateOnly SalaryToDate { get; set; }
    
    [Required]
    public DateOnly TitleFromDate { get; set; }
    
    [Required]
    public DateOnly TitleToDate { get; set; }
}