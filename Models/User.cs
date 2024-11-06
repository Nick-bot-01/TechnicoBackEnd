using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicoBackEnd.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string VATNum { get; set; }

    [Required]
    public required string Name { get; set; }

    [Required]
    public required string Surname { get; set; }
    
    [Required]
    public required string Phone { get; set; }

    [Required]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }

    public UserType Type { get; set; } = UserType.Owner;

    public List<Property> Properties { get; set; } = [];

    public bool IsActive { get; set; } = true;
}
public enum UserType { Owner, Admin };