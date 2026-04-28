using System.ComponentModel.DataAnnotations;

namespace Employees.IncomingDTO;

public class UpdateEmployeeDto
{
    [Required]
    public long Id { get; set; }

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
}