using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("employee", Schema = "employees")]
public class Employee
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [Column("birth_date")]
    public DateTime BirthDate { get; set; }

    [Required]
    [MaxLength(14)]
    [Column("first_name")]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(16)]
    [Column("last_name")]
    public string LastName { get; set; }

    [Required]
    [Column("gender")]
    public EmployeeGender Gender { get; set; }

    [Required]
    [Column("hire_date")]
    public DateTime HireDate { get; set; }
    
    public virtual ICollection<DepartmentEmployee> DepartmentEmployees { get; set; } = new List<DepartmentEmployee>();

    public virtual ICollection<DepartmentManager> DepartmentManagers { get; set; } = new List<DepartmentManager>();

    public virtual ICollection<Salary> Salaries { get; set; } = new List<Salary>();

    public virtual ICollection<Title> Titles { get; set; } = new List<Title>();
}

public enum EmployeeGender
{
    M,
    F,
}