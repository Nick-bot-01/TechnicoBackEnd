using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Responses;

namespace TechnicoBackEnd.Validators;
public class UserValidation : IUserValidation
{
    public ResponseApi<UserDTO>? UserValidator(UserDTO repair)
    {
        //Check Base properties
        return null; //todo implement
    }

    public ResponseApi<UserDTO>? UserValidator(UserWithPropertiesDTO repair)
    {
        ResponseApi<UserDTO>? result = UserValidator(repair);
        //Check Additional properties
        return null; //todo implement
    }

    public ResponseApi<UserDTO>? UserValidator(UserWithRequiredFieldsDTO repair)
    {
        ResponseApi<UserDTO>? result = UserValidator(repair);
        //Check Additional properties
        return null; //todo implement
    }

}
