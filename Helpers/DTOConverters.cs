using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Models;

namespace TechnicoBackEnd.Helpers;

public static class DTOConverters
{

    public static RepairDTO ConvertRepair(this Repair repair)
    {
        return new RepairDTO()
        {
            Id = repair.Id,
            Date = repair.Date,
            RType = repair.RType,
            Description = repair.Description,
            Status = repair.Status,
            Cost = repair.Cost,
            Property = repair.Property
        };
    }
}
