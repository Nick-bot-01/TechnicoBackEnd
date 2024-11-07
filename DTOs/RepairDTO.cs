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
    public required DateTime Date { get; set; }

    public required RepairType RType { get; set; }

    public required string Description { get; set; }

    public RepairStatus Status { get; set; }

    public required decimal Cost { get; set; }

    public required PropertyDTO Property { get; set; }
}
