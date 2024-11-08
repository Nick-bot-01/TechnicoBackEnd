using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Helpers;
using TechnicoBackEnd.Models;
using TechnicoBackEnd.Repositories;

public class PropertyService
{
    private readonly TechnicoDbContext db;

    public PropertyService(TechnicoDbContext db)
    {
        this.db = db;
    }

    public PropertyDTO? CreateProperty(PropertyDTO property)
    {
        if (string.IsNullOrWhiteSpace(property.PIN)) return null;
        if (string.IsNullOrWhiteSpace(property.Address)) return null;
        if (db.Properties.Any(x => x.PIN == property.PIN)) return null;
        if (property.ConstructionYear > DateTime.Now.Year) return null;

        Property dbproperty = new Property()
        {
            PIN = property.PIN,
            PType = (PropertyType)property.PType!,
            Address = property.Address!,
            Owner = property.Owner!,
            ConstructionYear = (int)property.ConstructionYear!
        };
        db.Properties.Add(dbproperty);
        db.SaveChanges();
        return dbproperty.ConvertProperty();
    }
    public List<PropertyDTO> GetAllProperties()
    {
        return db.Properties.Where(x => x.IsActive).Select(x => x.ConvertProperty()).ToList();
    }
    public PropertyDTO? GetPropertyById(int id)
    {
        return db.Properties.Where(x => x.Id == id && x.IsActive).Select(x => x.ConvertProperty()).FirstOrDefault();
    }
    public List<PropertyDTO> GetPropertiesByOwner(string vat)
    {
        return db.Properties.Where(x => x.OwnerVAT == vat && x.IsActive).Select(x => x.ConvertProperty()).ToList();
    }
    public List<PropertyDTO> SearchProperties(string? pin = null, string? vat = null)
    {
        if (pin is null && vat is null) return [];

        var results = db.Properties.Select(x => x);

        if (pin is not null) results = results.Where(x => x.PIN == pin);
        if (vat is not null) results = results.Where(x => x.OwnerVAT == vat);

        return results.Select(x => x.ConvertProperty()).ToList();
    }
    public PropertyDTO? UpdateProperty(PropertyDTO property)
    {
        if (string.IsNullOrWhiteSpace(property.PIN)) return null;
        if (string.IsNullOrWhiteSpace(property.Address)) return null;
        if (db.Properties.Any(x => x.PIN == property.PIN)) return null;
        if (property.ConstructionYear > DateTime.Now.Year) return null;

        Property? dbproperty = db.Properties.FirstOrDefault(x => x.Id == property.Id);
        if (dbproperty != null)
        {
            if (property.PIN != null) { dbproperty.PIN = property.PIN; }
            if (property.Address != null) { dbproperty.Address = property.Address; }
            if (property.Owner != null) { dbproperty.Owner = property.Owner; }
            if (property.ConstructionYear != null) { dbproperty.ConstructionYear = (int)property.ConstructionYear; }
            db.SaveChanges();
            return dbproperty.ConvertProperty();
        }
        return null;
    }
    public bool DeactivateProperty(int id)
    {
        Property? dbproperty = db.Properties.FirstOrDefault(x => x.Id == id);
        if (dbproperty != null)
        {
            foreach (var repair in dbproperty.Repairs)
            {
                repair.IsActive = false;
            }
            dbproperty.IsActive = false;
            db.SaveChanges();
            return true;
        }
        return false;
    }
    public bool DeleteProperty(int id)
    {
        Property? dbproperty = db.Properties.FirstOrDefault(x => x.Id == id);
        if (dbproperty != null)
        {
            db.Properties.Remove(dbproperty);
            db.SaveChanges();
            return true;
        }
        return false;
    }

}
