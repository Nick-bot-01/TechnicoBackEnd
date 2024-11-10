using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Models;
using TechnicoBackEnd.Responses;

namespace TechnicoBackEnd.Validators;
public class UserValidation : IUserValidation
{
    public ResponseApi<UserDTO>? UserValidator(UserDTO user)
    {
        if (string.IsNullOrWhiteSpace(user.VAT)) 
            return new ResponseApi<UserDTO> { Status = 1, Description = $"The user's vat must not be null, empty or whitespaces" };
        if (!IsAlphabeticalValid(user.Name)) 
            return new ResponseApi<UserDTO> { Status = 1, Description = $"The user's name must not be null, empty or whitespaces and must only contain alphabet characters" };
        if (!IsAlphabeticalValid(user.Surname)) 
            return new ResponseApi<UserDTO> { Status = 1, Description = $"The user's surname must not be null, empty or whitespaces and must only contain alphabet characters" };
        if (string.IsNullOrWhiteSpace(user.Address)) 
            return new ResponseApi<UserDTO> { Status = 1, Description = $"The user's address must not be null, empty or whitespaces" };
        if (!IsNumericalValid(user.Phone)) 
            return new ResponseApi<UserDTO> { Status = 1, Description = $"The user's phone must not be null, empty or whitespaces and must only contain numbers" };
        if (!IsEmailValid(user.Email)) 
            return new ResponseApi<UserDTO> { Status = 1, Description = $"The user's email must not be null, empty or whitespaces and it must be a valid email address" };
        return null;
    }

    public bool IsAlphabeticalValid(string? inputChar) 
    {

        if (string.IsNullOrWhiteSpace(inputChar)) return false;

        foreach (char c in inputChar)
        {
            if (!char.IsLetter(c)) return false;
        }
        return true;
    }

    public bool IsNumericalValid(string? inputNumber) => (long.TryParse(inputNumber, out long numberOutput)) ? true : false;

    public bool IsEmailValid(string? emailaddress)
    {
        if(string.IsNullOrWhiteSpace(emailaddress))
            return false;
        try
        {
            MailAddress m = new MailAddress(emailaddress);

            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    public ResponseApi<UserDTO>? UserValidator(UserWithPropertiesDTO user)
    {
        ResponseApi<UserDTO>? BaseResult = UserValidator(new UserDTO() {
            VAT = user.VAT,
            Name = user.Name,
            Address = user.Address,
            Phone = user.Phone,
            Surname = user.Surname,
            Email = user.Email
        });

        return BaseResult;
    }

    public ResponseApi<UserDTO>? UserValidator(UserWithRequiredFieldsDTO user)
    {
        ResponseApi<UserDTO>? BaseResult = UserValidator(new UserDTO()
        {
            VAT = user.VAT,
            Name = user.Name,
            Address = user.Address,
            Phone = user.Phone,
            Surname = user.Surname,
            Email = user.Email
        });

        if (BaseResult == null)
        {
            if (string.IsNullOrWhiteSpace(user.Password)) {
                return new ResponseApi<UserDTO> { Status = 1, Description = $"The user's password must not be null, empty or whitespaces" };
            }
        }
        return BaseResult;
    }

    public bool IsUserActive(User user) => user.IsActive;
}
