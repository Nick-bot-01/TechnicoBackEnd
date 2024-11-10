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
    public static PropertyDTO ConvertProperty(this Property property)
    {
        return new PropertyDTO()
        {
            Id = property.Id,
            PIN = property.PIN,
            Address = property.Address,
            ConstructionYear = property.ConstructionYear,
            PType = property.PType
        };
    }

    public static UserDTO ConvertUser(this User user){
        return new UserDTO()
        {
            Id = user.Id,
            VAT = user.VATNum,
            Name = user.Name,
            Surname = user.Surname,
            Address = user.Address,
            Phone = user.Phone,
            Email = user.Email,
        };
    }

    public static RepairDTO ConvertRepair(this Repair repair)
    {
        return new RepairDTO()
        {
            ScheduledDate = repair.ScheduledDate,
            RType = repair.RType,
            Description = repair.Description,
            Status = repair.Status,
            Cost = repair.Cost,
            PropertyIdNum = repair.Property.PIN
        };
    }

    public static AdminCreateUpdateRepairDTO ConvertRepairAdmin(this Repair repair)
    {
        return new AdminCreateUpdateRepairDTO()
        {
            ScheduledDate = repair.ScheduledDate,
            RType = repair.RType,
            Description = repair.Description,
            Status = repair.Status,
            Cost = repair.Cost,
            PropertyIdNum = repair.Property.PIN,
            OwnerVAT = repair.Property.Owner.VATNum
        };
    }
}
