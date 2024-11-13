using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TechnicoBackEnd.DTOs;
public class RepairDeactivateResponseDTO
{
    public int RepairId { get; set; }
    public bool IsActive { get; set; }
}
