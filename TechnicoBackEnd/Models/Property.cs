using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicoBackEnd.Models;

public class Property
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string PIN { get; set; }

    [Required]
    public required string Address { get; set; }

    public int ConstructionYear { get; set; }

    public PropertyType PType { get; set; } = PropertyType.Default;

    [Required]
    public required User Owner { get; set; }

    [NotMapped]
    public string OwnerVAT => Owner.VATNum;

    public List<Repair> Repairs { get; set; } = [];

    public bool IsActive { get; set; } = true;
}

public enum PropertyType { Default, Detached_house, Maisonet, Apartment_building };
