using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Responses;

namespace TechnicoBackEnd.Validators;
public class UserValidation : IUserValidation
{
    public ResponseApi<UserDTO>? UserValidator(UserDTO user)
    {
        if (string.IsNullOrWhiteSpace(user.VAT)) 
            return new ResponseApi<UserDTO> { Status = 1, Description = $"The user's vat must not be null, empty or whitespaces" };
        if (string.IsNullOrWhiteSpace(user.Name) && Regex.IsMatch(user.VAT, @"^[A-Za-z]+$")) 
            return new ResponseApi<UserDTO> { Status = 1, Description = $"The user's name must not be null, empty or whitespaces and must only contain alphabet characters" };
        if (string.IsNullOrWhiteSpace(user.Surname) && Regex.IsMatch(user.VAT, @"^[A-Za-z]+$")) 
            return new ResponseApi<UserDTO> { Status = 1, Description = $"The user's surname must not be null, empty or whitespaces and must only contain alphabet characters" };
        if (string.IsNullOrWhiteSpace(user.Address)) 
            return new ResponseApi<UserDTO> { Status = 1, Description = $"The user's address must not be null, empty or whitespaces" };
        if (string.IsNullOrWhiteSpace(user.Phone) && Regex.IsMatch(user.VAT, @"^\d+$")) 
            return new ResponseApi<UserDTO> { Status = 1, Description = $"The user's phone must not be null, empty or whitespaces and must only contain numbers" };
        if (string.IsNullOrWhiteSpace(user.Email) && Regex.IsMatch(user.VAT, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) 
            return new ResponseApi<UserDTO> { Status = 1, Description = $"The user's email must not be null, empty or whitespaces and it must be a valid email address" };
        return null; //todo implement
    }

    public ResponseApi<UserDTO>? UserValidator(UserWithPropertiesDTO user)
    {
        ResponseApi<UserDTO>? result = UserValidator(user);
        //Check Additional properties
        return null; //todo implement
    }

    public ResponseApi<UserDTO>? UserValidator(UserWithRequiredFieldsDTO user)
    {
        ResponseApi<UserDTO>? result = UserValidator(user);
        if (result == null && string.IsNullOrWhiteSpace(user.Password)) 
            return new ResponseApi<UserDTO> { Status = 1, Description = $"The user's password must not be null, empty or whitespaces" };
        return null; //todo implement
    }

}
