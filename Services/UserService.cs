using TechnicoBackEnd.Models;
using TechnicoBackEnd.Repositories;

namespace TechnicoBackEnd.Services;

public class UserService{
    private readonly TechnicoDbContext _dbContext;
    public UserService(TechnicoDbContext repairApplicationDbContext) => _dbContext = repairApplicationDbContext;

    public bool DeleteOwnerHard(User? user){
        if (user is null) return false;

        User? ownerQueryResult = _dbContext.Users.FirstOrDefault(c => c.VATNum == user.VATNum); //todo async
        if (ownerQueryResult == null) return false;

        //Delete user from the db
        _dbContext.Users.Remove(ownerQueryResult);
        _dbContext.SaveChanges(); //todo async
        return true;
    }

    public bool DeleteOwnerSoft(User? user){
        if (user is null) return false;

        User? ownerQueryResult = _dbContext.Users.FirstOrDefault(c => c.VATNum == user.VATNum); //todo async
        if (ownerQueryResult == null) return false;

        //Deactivate user from the db
        ownerQueryResult.IsActive = false;
        _dbContext.SaveChanges();//todo async
        return true;
    }

    public User? SearchUser(string? vat, string? email){
        if (vat == null && email == null) return null;
        User? userQuery = _dbContext.Users.Where(c => c.VATNum == vat || c.Email == email).FirstOrDefault(); //todo async
        return userQuery; //todo async
    }
}
