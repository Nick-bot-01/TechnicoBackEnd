using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Responses;

namespace TechnicoBackEnd.Validators;

public class PropertyValidation : IPropertyValidation
{
    public ResponseApi<PropertyDTO>? PropertyValidator(PropertyDTO property)
    {
        if (string.IsNullOrWhiteSpace(property.PIN))
            return new ResponseApi<PropertyDTO>
            {
                Status = 1,
                Description = "Property creation failed: No PIN given."
            };
        if (string.IsNullOrWhiteSpace(property.Address))
            return new ResponseApi<PropertyDTO>
            {
                Status = 1,
                Description = "Property creation failed: No Address given."
            };
        if (property.ConstructionYear > DateTime.Now.Year)
            return new ResponseApi<PropertyDTO>
            {
                Status = 1,
                Description = "Property creation failed: Invalid construction year."
            };
        return null;
    }
}
