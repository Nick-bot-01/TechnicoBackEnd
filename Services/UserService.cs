using Microsoft.EntityFrameworkCore;
using TechnicoBackEnd.Models;
using TechnicoBackEnd.Repositories;

namespace TechnicoBackEnd.Services;

public class UserService
{
    private readonly TechnicoDbContext _dbContext;
    public UserService(TechnicoDbContext repairApplicationDbContext) => _dbContext = repairApplicationDbContext;

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
