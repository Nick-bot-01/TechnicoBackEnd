using Microsoft.EntityFrameworkCore;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Helpers;
using TechnicoBackEnd.Models;
using TechnicoBackEnd.Repositories;
using TechnicoBackEnd.Responses;
using TechnicoBackEnd.Validators;

public class PropertyService : IPropertyService
{
    private readonly TechnicoDbContext db;
    private readonly IPropertyValidation val;

    public PropertyService(TechnicoDbContext db, IPropertyValidation val)
    {
        this.db = db;
        this.val = val;
    }

    public async Task<ResponseApi<PropertyDTO>> CreateProperty(PropertyDTO property)
    {
        var valResponse = val.PropertyValidator(property);
        
        if (valResponse != null) return valResponse;
        
        if (db.Properties.Any(x => x.PIN == property.PIN && x.IsActive))
            return new ResponseApi<PropertyDTO>
            {
                Status = 1,
                Description = "Property creation failed: Duplicate PIN."
            };

        User? owner = await db.Users.FirstOrDefaultAsync(x => x.Id == property.OwnerId && x.IsActive);

        if (owner is null)
            return new ResponseApi<PropertyDTO>
            {
                Status = 1,
                Description = $"Property creation failed: Owner with id {property.OwnerId} not found."
            };

        Property dbproperty = new Property()
        {
            PIN = property.PIN!,
            PType = (PropertyType)property.PType!,
            Address = property.Address!,
            Owner = owner,
            ConstructionYear = (int)property.ConstructionYear!
        };
        await db.Properties.AddAsync(dbproperty);
        await db.SaveChangesAsync();
        return new ResponseApi<PropertyDTO>
        {
            Status = 0,
            Description = "Property created succesfully.",
            Value = dbproperty.ConvertProperty()
        };
    }
    public async Task<ResponseApi<List<PropertyDTO>>> GetAllProperties()
    {
        var properties = await db.Properties.Include(x => x.Owner).Where(x => x.IsActive).Select(x => x.ConvertProperty()).ToListAsync();
        return new ResponseApi<List<PropertyDTO>>
        {
            Status = 0,
            Description = "Properties fetched successfully.",
            Value = properties
        };
    }
    public async Task<ResponseApi<PropertyDTO>> GetPropertyById(int id)
    {
        return new ResponseApi<PropertyDTO>
        {
            Status = 0,
            Description = $"Property with id {id} fetched succesfully.",
            Value = await db.Properties.Include(x => x.Owner).Where(x => x.Id == id && x.IsActive).Select(x => x.ConvertProperty()).FirstOrDefaultAsync()
        };
    }
    public async Task<ResponseApi<List<PropertyDTO>>> GetPropertiesByOwner(string vat)
    {
        var properties = await db.Properties.Include(x => x.Owner).Where(x => x.OwnerVAT == vat && x.IsActive).Select(x => x.ConvertProperty()).ToListAsync();
        return new ResponseApi<List<PropertyDTO>>
        {
            Status = 0,
            Description = $"Properties for owner with VAT {vat} fetched succesfully.",
            Value = properties
        };
    }
    public async Task<ResponseApi<List<PropertyDTO>>> SearchProperties(string? pin = null, string? vat = null)
    {
        if (pin is null && vat is null)
            return new ResponseApi<List<PropertyDTO>>
            {
                Status = 1,
                Description = "No search parameters.",
                Value = []
            };

        var results = db.Properties.Include(x => x.Owner).Select(x => x);

        if (pin is not null) results = results.Where(x => x.PIN == pin);
        if (vat is not null) results = results.Where(x => x.OwnerVAT == vat);

        var properties = await results.Select(x => x.ConvertProperty()).ToListAsync();

        return new ResponseApi<List<PropertyDTO>>
        {
            Status = 1,
            Description = "Search results fetched successfully.",
            Value = properties
        };
    }
    public async Task<ResponseApi<PropertyDTO>> UpdateProperty(PropertyDTO property)
    {
        if (db.Properties.Any(x => x.PIN == property.PIN && x.IsActive))
            return new ResponseApi<PropertyDTO>
            {
                Status = 1,
                Description = "Property update failed: Duplicate PIN."
            };
        if (property.ConstructionYear > DateTime.Now.Year)
            return new ResponseApi<PropertyDTO>
            {
                Status = 1,
                Description = "Property update failed: Invalid construction year."
            };

        Property? dbproperty = db.Properties.FirstOrDefault(x => x.Id == property.Id);
        if (dbproperty != null)
        {
            if (property.PIN != null) { dbproperty.PIN = property.PIN; }
            if (property.Address != null) { dbproperty.Address = property.Address; }
            if (property.ConstructionYear != null) { dbproperty.ConstructionYear = (int)property.ConstructionYear; }
            await db.SaveChangesAsync();
            return new ResponseApi<PropertyDTO>
            {
                Status = 1,
                Description = $"Property update failed : Property with id {property.Id} not found.",
                Value = dbproperty.ConvertProperty()
            };
        }
        return new ResponseApi<PropertyDTO>
        {
            Status = 1,
            Description = $"Property update failed : Property with id {property.Id} not found."
        };
    }
    public async Task<ResponseApi<PropertyDTO>> DeactivateProperty(int id)
    {
        Property? dbproperty = await db.Properties.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
        if (dbproperty != null)
        {
            foreach (var repair in dbproperty.Repairs)
            {
                repair.IsActive = false;
            }
            dbproperty.IsActive = false;
            await db.SaveChangesAsync();
            return new ResponseApi<PropertyDTO>()
            {
                Status = 0,
                Description = $"Property with id {id} deactivated succesfully."
            };
        }
        return new ResponseApi<PropertyDTO>()
        {
            Status = 1,
            Description = $"Deactivation failed: Property with id {id} not found."
        };
    }
    public async Task<ResponseApi<PropertyDTO>> DeleteProperty(int id)
    {
        Property? dbproperty = await db.Properties.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
        if (dbproperty != null)
        {
            db.Properties.Remove(dbproperty);
            await db.SaveChangesAsync();
            return new ResponseApi<PropertyDTO>()
            {
                Status = 0,
                Description = $"Property with id {id} deleted succesfully."
            };
        }
        return new ResponseApi<PropertyDTO>()
        {
            Status = 1,
            Description = $"Deletion failed: Property with id {id} not found."
        };
    }

}
