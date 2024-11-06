using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TechnicoBackEnd.Models;
using TechnicoBackEnd.Repositories;
using TechnicoBackEnd.ResponsesGT;

namespace TechnicoBackEnd.Services;

public class RepairService
{
    private readonly TechnicoDbContext db;

    public RepairService(TechnicoDbContext db) // Dependency Injection
    {
        this.db = db;
    }

    public ResponseApi<Repair> CreateRepair(Repair repair, int propertyId)
    {
        // check if the property item exists
        var propertyItem = db.Properties.FirstOrDefault(p => p.Id == propertyId);
        if (propertyItem == null)
            return new ResponseApi<Repair> { Status = 1, Description = $"Repair creation failed. Property with id {propertyId} was not found." };

        // Validate required fields
        if (repair.ScheduledDate == default || repair.ScheduledDate < DateTime.Now.AddHours(1) || repair.RType == RepairType.Other ||
            string.IsNullOrWhiteSpace(repair.Description) || repair.Cost <= 0)
            return new ResponseApi<Repair> { Status = 1, Description = $"Creation of repair for property with id {propertyId} failed. All Repair fields must be filled." };

        // Set propertyitemid col for the repair entry - EF will connect the dots
        repair.Property = propertyItem;

        try
        {
            db.Repairs.Add(repair);
            db.SaveChanges();
            return new ResponseApi<Repair> { Status = 0, Description = $"Repair for property with id {propertyId} was created successfully.", Value = repair };
        }
        catch (Exception e)
        {
            return new ResponseApi<Repair> { Status = 1, Description = $"Repair creation for property with id {propertyId} failed due to a database error: '{e.Message}'" };
        }
    }

    public ResponseApi<Repair> UpdateRepair(Repair updatedRepair)
    {
        var repairDb = db.Repairs.FirstOrDefault(r => r.Id == updatedRepair.Id);

        if (repairDb == null)
            return new ResponseApi<Repair> { Status = 1, Description = $"No repair with this id was found in the database." };

        // Updating fields if validations pass - only update field when it has different value from db entry and not null
        repairDb.ScheduledDate = (updatedRepair.ScheduledDate != default && updatedRepair.ScheduledDate >= DateTime.Now.AddHours(1)) ? updatedRepair.ScheduledDate : repairDb.ScheduledDate;
        repairDb.RType = (updatedRepair.RType != RepairType.Other && updatedRepair.RType != repairDb.RType) ? updatedRepair.RType : repairDb.RType;
        repairDb.Description = !string.IsNullOrWhiteSpace(updatedRepair.Description) ? updatedRepair.Description : repairDb.Description;
        repairDb.Status = updatedRepair.Status != repairDb.Status ? updatedRepair.Status : repairDb.Status;
        repairDb.Cost = updatedRepair.Cost > 0 ? updatedRepair.Cost : repairDb.Cost;

        try
        {
            db.SaveChanges();
            return new ResponseApi<Repair> { Status = 0, Description = $"Repair with id {repairDb.Id} was updated successfully.", Value = updatedRepair };
        }
        catch (Exception e)
        {
            return new ResponseApi<Repair> { Status = 1, Description = $"Repair update for repair with id {repairDb.Id} failed due to a database error: '{e.Message}'" };
        }

    }

    public ResponseApi<Repair> DeleteRepair(Repair repairTBD, bool softDelete = true)
    {
        var repair = db.Repairs.FirstOrDefault(r => r.Id == repairTBD.Id);

        if (repair == null)
            return new ResponseApi<Repair> { Status = 1, Description = $"No repair with this id was found in the database." };

        if (softDelete)
        {
            //repair.isActive = true;
            db.SaveChanges();
            return new ResponseApi<Repair> { Status = 0, Description = $"Repair with id {repair.Id} was deactivated successfully." };
        }
        else
        {
            db.Repairs.Remove(repair);
            db.SaveChanges();
            return new ResponseApi<Repair> { Status = 0, Description = $"Repair with id {repair.Id} was deleted successfully." };
        }
    }
}
