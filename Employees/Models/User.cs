using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employees.Models;

using Microsoft.AspNetCore.Identity;

public class User: IdentityUser
{

    [Required]
    [MaxLength(255)]
    public string Name { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string LastName { get; set; }
}