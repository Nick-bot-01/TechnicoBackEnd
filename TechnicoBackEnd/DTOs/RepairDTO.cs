using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnicoBackEnd.Models;

namespace TechnicoBackEnd.DTOs;
public class RepairDTO
{
    public int Id { get; set; }
    public DateTime ScheduledDate { get; set; }
    public RepairType RType { get; set; }
    public string? Description { get; set; }
    public RepairStatus Status { get; set; }
    public decimal Cost { get; set; }
    public string? PropertyIdNum { get; set; }
}
