using Microsoft.EntityFrameworkCore;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Helpers;
using TechnicoBackEnd.Models;
using TechnicoBackEnd.Repositories;
using TechnicoBackEnd.Responses;


namespace TechnicoBackEnd.Services;

public class UserService : IUserService
{
    private readonly TechnicoDbContext _dbContext;
    public UserService(TechnicoDbContext repairApplicationDbContext) => _dbContext = repairApplicationDbContext;

    public async Task<ResponseApi<UserDTO>> DeleteOwnerHard(string? vat)
    {
        if (string.IsNullOrEmpty(vat)) return new ResponseApi<UserDTO> { Status = 1, Description = "Input argument null or empty" };

        User? ownerQueryResult = await _dbContext.Users.FirstOrDefaultAsync(c => c.VATNum == vat);
        if (ownerQueryResult == null) return new ResponseApi<UserDTO> { Status = 1, Description = "User Not Found" };

        //Check if already inactive
        if(ownerQueryResult.IsActive == false) return new ResponseApi<UserDTO> { Status = 1, Description = "User Not Found" };

        //Delete user from the db
        _dbContext.Users.Remove(ownerQueryResult);
        await _dbContext.SaveChangesAsync();
        return new ResponseApi<UserDTO> { Status = 0, Description = $"User with Vat: {ownerQueryResult.VATNum} has been removed!" };
    }

    public async Task<ResponseApi<UserDTO>> DeleteOwnerSoft(string? vat)
    {
        if (string.IsNullOrEmpty(vat)) return new ResponseApi<UserDTO> { Status = 1, Description = "Input argument null or empty" };

        User? ownerQueryResult = await _dbContext.Users.FirstOrDefaultAsync(c => c.VATNum == vat);
        if (ownerQueryResult == null) return new ResponseApi<UserDTO> { Status = 1, Description = "User Not Found" };

        //Check if already inactive
        if (ownerQueryResult.IsActive == false) return new ResponseApi<UserDTO> { Status = 1, Description = "User Not Found" };

        //Deactivate user from the db
        ownerQueryResult.IsActive = false;
        await _dbContext.SaveChangesAsync();
        return new ResponseApi<UserDTO> { Status = 0, Description = $"User with Vat: {ownerQueryResult.VATNum} has been removed!" };
    }

    public async Task<ResponseApi<UserDTO>> SearchUser(string? vat, string? email)
    {
        if (vat == null && email == null) return new ResponseApi<UserDTO> { Status = 1, Description = "Vat and Email arguments not found!" };

        User? userQuery = await _dbContext.Users.Where(c => c.VATNum == vat || c.Email == email).FirstOrDefaultAsync();

        if (userQuery != null && userQuery.IsActive == true) return new ResponseApi<UserDTO> { Status = 0, Description = "User has been found!", Value = userQuery.ConvertUser() };
        else return new ResponseApi<UserDTO> { Status = 1, Description = "User Not Found" };
    }

    public async Task<ResponseApi<UserDTO>> Register(UserWithRequiredFieldsDTO userDto)
    {
        //checks if user input was given - maybe remove?
        if (userDto is null) return new ResponseApi<UserDTO> { Status = 1, Description = $"User creation failed. No user input was given" };

        //checks if user exists
        var existingUserQuery = _dbContext.Users.FirstOrDefault(o => o.VATNum == userDto.VAT);
        if (existingUserQuery != null) return new ResponseApi<UserDTO> { Status = 1, Description = $"User creation with vat {userDto.VAT} failed. User already exists" };

        //checks if values are "" or " "
        if (string.IsNullOrWhiteSpace(userDto.VAT) ||
            string.IsNullOrWhiteSpace(userDto.Name) ||
            string.IsNullOrWhiteSpace(userDto.Surname) ||  //check if name or surnmae has numbers
            string.IsNullOrWhiteSpace(userDto.Address) ||
            string.IsNullOrWhiteSpace(userDto.Phone) ||    //check if phone has alphabet characters
            string.IsNullOrWhiteSpace(userDto.Email) ||    //check if email has actual email structure
            string.IsNullOrWhiteSpace(userDto.Password))    //add regex or find better solution to check users credentials validity - add address
            return new ResponseApi<UserDTO> { Status = 1, Description = $"User creation with vat {userDto.VAT} failed. The required fields must not be null, empty or whitespaces" };

        var user = new User
        {
            VATNum = userDto.VAT,
            Name = userDto.Name,
            Surname = userDto.Surname,
            Address = userDto.Address,
            Phone = userDto.Phone,
            Email = userDto.Email,
            Password = userDto.Password
        };

        try
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return new ResponseApi<UserDTO> { Status = 0, Description = $"User with vat {userDto.VAT} was created successfully.", Value = user.ConvertUser() };
        }
        catch (Exception e)
        {
            return new ResponseApi<UserDTO> { Status = 1, Description = $"User creation with vat {userDto.VAT} failed. Probable database error with message : '{e.Message}'" };
        }
    }

    public async Task<ResponseApi<UserDTO>> GetUserDetailsById(int id)
    {
        //returns the userdto with the specified id that is active
        var user = await _dbContext.Users.Where(u => u.IsActive).FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return new ResponseApi<UserDTO> { Status = 1, Description = $"User search with id {id} failed. No user was found" };
        return new ResponseApi<UserDTO> { Status = 0, Description = "", Value = user.ConvertUser() };

    }

    public async Task<ResponseApi<List<UserDTO>>> GetAllUsers()
    {
        //returns all active users to a userdto list
        var users = await _dbContext.Users.Where(u => u.IsActive).ToListAsync();
        return new ResponseApi<List<UserDTO>> { Status = 0, Description = "", Value = users.Select(user => user.ConvertUser()).ToList() };
    }

}
