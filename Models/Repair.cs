using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TechnicoBackEnd.Models;

public class Repair
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime ScheduledDate { get; set; }

    [Required]
    public required RepairType RType { get; set; }

    [Required]
    [Column(TypeName = "ntext")]
    public required string Description { get; set; }

    public RepairStatus Status { get; set; } = RepairStatus.Pending;

    [Required]
    [Precision(8,2)]
    public required decimal Cost { get; set; }

    [Required]
    public required Property Property { get; set; }

    [NotMapped]
    public User Owner => Property.Owner;

    [NotMapped]
    public string Address => Property.Address;

    public bool IsActive { get; set; } = true;
}

public enum RepairType { Other, Painting, Insulation, Frames, Plumbing, Electrical };
public enum RepairStatus { Pending, In_Progress, Complete };