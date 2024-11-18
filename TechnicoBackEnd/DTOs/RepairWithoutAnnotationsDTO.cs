using TechnicoBackEnd.Models;

namespace TechnicoBackEnd.DTOs;
public class RepairWithoutAnnotationsDTO
{
    public int Id { get; set; }

    public DateTime ScheduledDate { get; set; }

    public RepairType RType { get; set; }

    public string? Description { get; set; }

    public RepairStatus Status { get; set; } = RepairStatus.Pending;

    public decimal Cost { get; set; }

    public string? PropertyIdNum { get; set; }

    public string? PropertyAddress { get; set; }

    public string? OwnerVAT { get; set; }

    public string? ErrorDescription { get; set; }

    public int? ErrorCode { get; set; }
}
