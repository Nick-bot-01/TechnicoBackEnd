using Microsoft.EntityFrameworkCore;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Helpers;
using TechnicoBackEnd.Models;
using TechnicoBackEnd.Repositories;
using TechnicoBackEnd.ResponsesGT;


namespace TechnicoBackEnd.Services;

public class UserService{
    private readonly TechnicoDbContext _dbContext;
    public UserService(TechnicoDbContext repairApplicationDbContext) => _dbContext = repairApplicationDbContext;

    public async Task<ResponseApi<UserDTO>> DeleteOwnerHard(string? vat){
        if (vat is null || vat == string.Empty) return new ResponseApi<UserDTO> { Status = 1, Description = "Input argument null or empty" };

        User? ownerQueryResult = await _dbContext.Users.FirstOrDefaultAsync(c => c.VATNum == vat); //todo async
        if (ownerQueryResult == null) return new ResponseApi<UserDTO> { Status = 1, Description = "User Not Found" };

        //Delete user from the db
        _dbContext.Users.Remove(ownerQueryResult);
        await _dbContext.SaveChangesAsync(); //todo async
        return new ResponseApi<UserDTO> { Status = 0, Description = $"User with Vat: {ownerQueryResult.VATNum} has been removed!" };
    }

    public async Task<ResponseApi<UserDTO>> DeleteOwnerSoft(string? vat){
        if (vat is null || vat == string.Empty) return new ResponseApi<UserDTO> { Status = 1, Description = "Input argument null or empty" };

        User? ownerQueryResult = await _dbContext.Users.FirstOrDefaultAsync(c => c.VATNum == vat);
        if (ownerQueryResult == null) return new ResponseApi<UserDTO> {Status = 1, Description = "User Not Found" };

        //Deactivate user from the db
        ownerQueryResult.IsActive = false;
        await _dbContext.SaveChangesAsync();
        return new ResponseApi<UserDTO> { Status = 0, Description = $"User with Vat: {ownerQueryResult.VATNum} has been removed!"};
    }

    public async Task<ResponseApi<UserDTO>> SearchUser(string? vat, string? email){
        if (vat == null && email == null) return new ResponseApi<UserDTO> { Status = 1, Description = "Vat and Email arguments not found!" };

        User? userQuery = await _dbContext.Users.Where(c => c.VATNum == vat || c.Email == email).FirstOrDefaultAsync(); //todo async
        
        if(userQuery != null && userQuery.IsActive == true) return new ResponseApi<UserDTO> { Status = 0, Description = "User has been found!", Value = userQuery.ConvertUser()};
        else return new ResponseApi<UserDTO> { Status = 1, Description = "User Not Found" };
    }

    public User? Register(User? user) //change argument to dto
    {
        if (user is null) return null;

        var existingUserQuery = _dbContext.Users.FirstOrDefault(o => o.VATNum == user.VATNum);  //checks if user exists
        if (existingUserQuery != null) return null;

        if (string.IsNullOrWhiteSpace(user.VATNum) ||  //checks if values are "" or " "
            string.IsNullOrWhiteSpace(user.Name) ||  //important - add address?
            string.IsNullOrWhiteSpace(user.Surname) ||  //check if name or surnmae has numbers
            string.IsNullOrWhiteSpace(user.Phone) ||    //check if phone has alphabet characters
            string.IsNullOrWhiteSpace(user.Email) ||    //check if email has actual email structure
            string.IsNullOrWhiteSpace(user.Password))
            return null;

        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();   //make async
        return user;
    }

    public User? GetUserDetailsById(int id) //change argument to dto
    {
        return _dbContext.Users.Where(u => u.IsActive == true).FirstOrDefault(u => u.Id == id);   //make async
    }

    public List<User> GetAllUsers()
    {
        return _dbContext.Users.Where(u => u.IsActive == true).ToList();  //make async
    }
    
}
