using Microsoft.EntityFrameworkCore;
using TechnicoBackEnd.Models;
using TechnicoBackEnd.Repositories;

namespace TechnicoBackEnd.Services;

public class UserService{
    private readonly TechnicoDbContext _dbContext;
    public UserService(TechnicoDbContext repairApplicationDbContext) => _dbContext = repairApplicationDbContext;

    public bool DeleteOwnerHard(string? vat)
    {
        if (vat is null || vat == string.Empty) return false;

        User? ownerQueryResult = _dbContext.Users.FirstOrDefault(c => c.VATNum == vat); //todo async
        if (ownerQueryResult == null) return false;

        //Delete user from the db
        _dbContext.Users.Remove(ownerQueryResult);
        _dbContext.SaveChanges(); //todo async
        return true;
    }

    public bool DeleteOwnerSoft(string? vat){
        if (vat is null || vat == string.Empty) return false;

        User? ownerQueryResult = _dbContext.Users.FirstOrDefault(c => c.VATNum == vat); //todo async
        if (ownerQueryResult == null) return false;

        //Deactivate user from the db
        ownerQueryResult.IsActive = false;
        _dbContext.SaveChanges();//todo async
        return true;
    }

    public User? SearchUser(string? vat, string? email){
        if (vat == null && email == null) return null;
        User? userQuery = _dbContext.Users.Where(c => c.VATNum == vat || c.Email == email).FirstOrDefault(); //todo async
        if(userQuery != null && userQuery.IsActive == true) return userQuery; //todo async
        else return null;
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
