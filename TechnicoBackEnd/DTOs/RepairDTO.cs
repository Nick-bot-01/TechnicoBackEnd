using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnicoBackEnd.Models;

namespace TechnicoBackEnd.DTOs;
public class RepairDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Scheduled date is required.")]
    public DateTime ScheduledDate { get; set; }

    [Required(ErrorMessage = "Repair type is required.")]
    public RepairType RType { get; set; }

    [Required(ErrorMessage = "Repair description is required.")]
    public required string Description { get; set; }

    public RepairStatus Status { get; set; } = RepairStatus.Pending;

    [Range(0.01, 1000000, ErrorMessage = "Cost must be greater than 0.")]
    public decimal Cost { get; set; }

    [Required(ErrorMessage = "Property identification number (PIN) is required.")]
    public string? PropertyIdNum { get; set; }

    public string? PropertyAddress { get; set; }

    public string? OwnerVAT { get; set; }

    public string? ErrorDescription { get; set; }

    public int? ErrorCode { get; set; }
}
