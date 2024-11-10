using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Responses;

namespace TechnicoBackEnd.Validators
{
    public interface IUserValidation
    {
        ResponseApi<UserDTO>? UserValidator(UserDTO repair);
        ResponseApi<UserDTO>? UserValidator(UserWithPropertiesDTO repair);
        ResponseApi<UserDTO>? UserValidator(UserWithRequiredFieldsDTO repair);
    }
}