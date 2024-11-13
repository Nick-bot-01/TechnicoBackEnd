using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Responses;

namespace TechnicoBackEnd.Validators
{
    public interface IPropertyValidation
    {
        ResponseApi<PropertyDTO>? PropertyValidator(PropertyDTO property);
    }
}