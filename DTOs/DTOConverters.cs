using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnicoBackEnd.Models;

namespace TechnicoBackEnd.DTOs;
public static class DTOConverters
{
    public static RepairDTO ConvertRepair(this Repair repair)
    {
        return new RepairDTO
        {
            Id = repair.Id,
            ScheduledDate = repair.ScheduledDate,
            RType = repair.RType,
            Description = repair.Description,
            Status = repair.Status,
            Cost = repair.Cost,
            PropertyId = repair.Property.Id,
            OwnerName = repair.Property.Owner.Name, // Assuming Owner has a Name property
            Address = repair.Property.Address
        };
    }
}
