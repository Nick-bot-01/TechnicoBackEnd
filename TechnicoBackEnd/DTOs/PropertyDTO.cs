using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnicoBackEnd.Models;

namespace TechnicoBackEnd.DTOs;

public class PropertyDTO
{
    public int Id { get; set; }

    public string? PIN { get; set; }

    public string? Address { get; set; }

    public int? ConstructionYear { get; set; }

    public PropertyType? PType { get; set; }

    public int? OwnerId { get; set; }
}
