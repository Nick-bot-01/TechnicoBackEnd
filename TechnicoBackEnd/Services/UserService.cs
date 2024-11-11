using Microsoft.EntityFrameworkCore;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Helpers;
using TechnicoBackEnd.Models;
using TechnicoBackEnd.Repositories;
using TechnicoBackEnd.Responses;
using TechnicoBackEnd.Validators;


namespace TechnicoBackEnd.Services;

public class UserService : IUserService
{
    private readonly TechnicoDbContext _dbContext;
    public UserService(TechnicoDbContext repairApplicationDbContext) => _dbContext = repairApplicationDbContext;

    public async Task<ResponseApi<UserDTO>> DeleteHard(string? vat){
        UserValidation userValidation = new();

        if (string.IsNullOrEmpty(vat)) return new ResponseApi<UserDTO> { Status = 1, Description = "Input argument null or empty" };

        User? ownerQueryResult = await _dbContext.Users.FirstOrDefaultAsync(c => c.VATNum == vat);
        if (GenericValidation.IsNull(ownerQueryResult).Value) return new ResponseApi<UserDTO> { Status = 1, Description = "User Not Found" };

        //Check if already inactive
        if(!userValidation.IsUserActive(ownerQueryResult!)) return new ResponseApi<UserDTO> { Status = 1, Description = "User Not Found" };

        //Delete user from the db
        _dbContext.Users.Remove(ownerQueryResult!);
        await _dbContext.SaveChangesAsync();
        return new ResponseApi<UserDTO> { Status = 0, Description = $"User with Vat: {ownerQueryResult!.VATNum} has been removed!" };
    }

    public async Task<ResponseApi<UserDTO>> DeleteSoft(string? vat){
        UserValidation userValidation = new();
        if (string.IsNullOrEmpty(vat)) return new ResponseApi<UserDTO> { Status = 1, Description = "Input argument null or empty" };

        User? ownerQueryResult = await _dbContext.Users.FirstOrDefaultAsync(c => c.VATNum == vat);
        if (GenericValidation.IsNull(ownerQueryResult).Value) return new ResponseApi<UserDTO> { Status = 1, Description = "User Not Found" };

        //Check if already inactive
        if (!userValidation.IsUserActive(ownerQueryResult!)) return new ResponseApi<UserDTO> { Status = 1, Description = "User Not Found" };

        //Deactivate user from the db
        ownerQueryResult!.IsActive = false;
        await _dbContext.SaveChangesAsync();
        return new ResponseApi<UserDTO> { Status = 0, Description = $"User with Vat: {ownerQueryResult.VATNum} has been removed!" };
    }

    public async Task<ResponseApi<UserDTO>> Search(string? vat, string? email){
        UserValidation userValidation =new();

        if (string.IsNullOrEmpty(vat) && string.IsNullOrEmpty(email)) 
            return new ResponseApi<UserDTO> { Status = 1, Description = "Vat and Email arguments not found!" };

        User? userQuery = await _dbContext.Users.Where(c => c.VATNum == vat || c.Email == email).FirstOrDefaultAsync();

        if (!GenericValidation.IsNull(userQuery).Value && userValidation.IsUserActive(userQuery!)) 
            return new ResponseApi<UserDTO> { Status = 0, Description = "User has been found!", Value = userQuery!.ConvertUser() };
        else 
            return new ResponseApi<UserDTO> { Status = 1, Description = "User Not Found" };
    }

    public async Task<ResponseApi<UserDTO>> Register(UserWithRequiredFieldsDTO userDto)
    {
        //checks if user input was given
        if (GenericValidation.IsNull(userDto).Value) return new ResponseApi<UserDTO> { Status = 1, Description = $"User creation failed. No user input was given" };

        //checks if user exists
        var existingUserQuery = await _dbContext.Users.FirstOrDefaultAsync(o => o.VATNum == userDto.VAT);
        if (!GenericValidation.IsNull(existingUserQuery).Value) 
            return new ResponseApi<UserDTO> { Status = 1, Description = $"User creation failed. User already exists" };

        UserValidation uservalidation = new();
        ResponseApi<UserDTO>? validationResponse = uservalidation.UserValidator(userDto);
        if (validationResponse != null) return validationResponse;

        var user = new User
        {
            VATNum = userDto.VAT!,
            Name = userDto.Name!,
            Surname = userDto.Surname!,
            Address = userDto.Address!,
            Phone = userDto.Phone!,
            Email = userDto.Email!,
            Password = userDto.Password!
        };

        try
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return new ResponseApi<UserDTO> { Status = 0, Description = $"User with vat {userDto.VAT} was created successfully.", Value = user.ConvertUser() };
        }
        catch (Exception e)
        {
            return new ResponseApi<UserDTO> { Status = 1, Description = $"User creation failed. Probable database error with message : '{e.Message}'" };
        }
    }

    public async Task<ResponseApi<UserDTO>> GetUserDetailsById(int id){
        //returns the userdto with the specified id that is active
        var user = await _dbContext.Users.Where(u => u.IsActive).FirstOrDefaultAsync(u => u.Id == id);
        if (GenericValidation.IsNull(user).Value) 
            return new ResponseApi<UserDTO> { Status = 1, Description = $"User search with id {id} failed. No user was found" };

        return new ResponseApi<UserDTO> { Status = 0, Description = "", Value = user!.ConvertUser() };

    }

    public async Task<ResponseApi<List<UserDTO>>> GetAllUsers(){
        //returns all active users to a userdto list
        var users = await _dbContext.Users.Where(u => u.IsActive).ToListAsync();
        return new ResponseApi<List<UserDTO>> { Status = 0, Description = "", Value = users.Select(user => user.ConvertUser()).ToList() };
    }

    public async Task<ResponseApi<UserDTO>> Update(UserWithRequiredFieldsDTO userDto)
    {
        //Input argument check
        if (GenericValidation.IsNull(userDto).Value) return new ResponseApi<UserDTO> { Status = 1, Description = $"User update failed. No user input was given" };

        //User Exists
        var existingUserQuery = await _dbContext.Users.FirstOrDefaultAsync(o => o.VATNum == userDto.VAT);
        if (GenericValidation.IsNull(existingUserQuery).Value)
            return new ResponseApi<UserDTO> { Status = 1, Description = $"No user found." };

        UserValidation uservalidation = new();

        var user = new User
        {
            VATNum = existingUserQuery!.VATNum,
            Name = (uservalidation.IsAlphabeticalValid(userDto.Name))? userDto.Name! : existingUserQuery.Name,
            Surname = (uservalidation.IsAlphabeticalValid(userDto.Surname)) ? userDto.Surname! : existingUserQuery.Surname,
            Address = (string.IsNullOrEmpty(userDto.Address))? existingUserQuery.Address :userDto.Address!,
            Phone = (uservalidation.IsNumericalValid(userDto.Phone)) ? userDto.Phone! : existingUserQuery.Phone,
            Email = existingUserQuery!.Email,
            Password = (string.IsNullOrEmpty(userDto.Password)) ? existingUserQuery.Password : userDto.Password!
        };

        try
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return new ResponseApi<UserDTO> { Status = 0, Description = $"User with vat {userDto.VAT} was updated successfully.", Value = user.ConvertUser() };
        }
        catch (Exception e)
        {
            return new ResponseApi<UserDTO> { Status = 1, Description = $"User update failed. Probable database error with message : '{e.Message}'" };
        }
    }
}
