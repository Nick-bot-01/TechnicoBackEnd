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
            PType = property.PType,
            Owner = property.Owner
        };
    }
}
