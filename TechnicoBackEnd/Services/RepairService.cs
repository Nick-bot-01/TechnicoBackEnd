using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Helpers;
using TechnicoBackEnd.Models;
using TechnicoBackEnd.Repositories;
using TechnicoBackEnd.Responses;
using TechnicoBackEnd.Validators;

namespace TechnicoBackEnd.Services;

public class RepairService : IRepairService
{
    private readonly TechnicoDbContext db;
    private readonly IRepairValidation validation;

    public RepairService(TechnicoDbContext db, IRepairValidation validation) // Dependency Injection
    {
        this.db = db;
        this.validation = validation;
    }

    public async Task<ResponseApi<List<RepairDTO>>> GetAllOwnerRepairsByDateOrRangeOfDates(DateTime StartDate, DateTime EndDate)
    {
        if (EndDate != DateTime.MinValue && EndDate < StartDate)
        {
            return new ResponseApi<List<RepairDTO>> { Status = 1, Description = $"Please enter valid Dates." };
        }

        EndDate = (EndDate == DateTime.MinValue) ? DateTime.MaxValue : EndDate;

        var GetQuery2 = await db.Repairs
            .Include(r => r.Property)
            .ThenInclude(r => r.Owner)
            .Where(r => r.ScheduledDate >= StartDate && r.ScheduledDate <= EndDate)
            .Select(r => r.ConvertRepair())
            .ToListAsync();

        return new ResponseApi<List<RepairDTO>> { Description = $"Repairs between specified dates were found.", Value = GetQuery2, Status = 0 };
    }
    public async Task<ResponseApi<List<RepairDTO>>> GetAllOwnerRepairsByUID(int id)
    {
        if (id <= 0)
            return new ResponseApi<List<RepairDTO>> { Status = 1, Description = $"The VAT Number you entered is not valid. " };

        var GetQuery = await db.Repairs
            .Include(r => r.Property)
            .ThenInclude(r => r.Owner)
            .Where(r => r.Property.Owner.Id == id)
            .Select(r => r.ConvertRepair())
            .ToListAsync();

        if (GetQuery.Count == 0)
        {
            return new ResponseApi<List<RepairDTO>> { Status = 1, Description = $"There are no repairs for owner with ID: {id} in the database. " };
        }

        try
        {
            return new ResponseApi<List<RepairDTO>> { Value = GetQuery, Status = 0, Description = $"List of repairs with ID: {id} created." };
        }
        catch (Exception e)
        {
            return new ResponseApi<List<RepairDTO>> { Status = 1, Description = $"The list of repairs with ID: {id} failed to create due to a database error: '{e.Message}'" };
        }
    }

    public async Task<ResponseApi<RepairDTO>> GetRepairByID(int id)
    {
        var GetQuery = await db.Repairs
            .Include(r => r.Property)
            .ThenInclude(r => r.Owner)
            .Where(r => r.Id == id)
            .Select(r => r.ConvertRepair())
            .FirstOrDefaultAsync();

        Console.WriteLine(GetQuery);

        if (GetQuery == null)
        {
            return new ResponseApi<RepairDTO> { Status = 1, Description = $"Repair with id: {id} was not found." };
        }

        return new ResponseApi<RepairDTO> { Status = 0, Value = GetQuery, Description = $"Repair with id: {id} was found." };
    }
    public async Task<ResponseApi<RepairDTO>> CreateRepairUser(RepairDTO repairDto)
    {
        if (repairDto == null) return new ResponseApi<RepairDTO> { Status = 1, Description = $"Repair creation failed. The DTO argument that was given is null." };

        // Check if the property intended for repair exists in the db
        var propertyItem = await db.Properties.FirstOrDefaultAsync(p => p.PIN == repairDto.PropertyIdNum);
        if (propertyItem == null)
            return new ResponseApi<RepairDTO> { Status = 1, Description = $"Repair creation failed. Property with PIN {repairDto.PropertyIdNum} was not found." };

        // Validate the user input values
        var validationResponse = validation.RepairValidatorUser(repairDto);
        if (validationResponse != null)
        {
            return validationResponse;
        }

        // Map DTO to a new Repair entity (which is the one that ll be saved in the db, it having all the relationships, validations etc in contrast to the dto)
        var repair = new Repair
        {
            ScheduledDate = repairDto.ScheduledDate,
            RType = repairDto.RType,
            Description = repairDto.Description!,
            Status = RepairStatus.Pending,
            Cost = repairDto.Cost,
            Property = propertyItem // Set PropertyId field value for the new repair db entry - EF will connect the dots
        };

        try
        {
            await db.Repairs.AddAsync(repair);
            await db.SaveChangesAsync();
            //repair.Property.Owner.Id = propertyItem.Owner.Id;
            return new ResponseApi<RepairDTO> { Status = 0, Description = $"Repair for property with PIN {repairDto.PropertyIdNum} was created successfully.", Value = repair.ConvertRepair() };
        }
        catch (Exception e)
        {
            return new ResponseApi<RepairDTO> { Status = 1, Description = $"Repair creation for property with PIN {repairDto.PropertyIdNum} failed due to a database error: '{e.Message}'" };
        }
    }

    public async Task<ResponseApi<RepairAdminCreateUpdateDTO>> CreateRepairAdmin(RepairAdminCreateUpdateDTO repairDto)
    {
        if (repairDto == null) 
            return new ResponseApi<RepairAdminCreateUpdateDTO> { Status = 1, Description = $"Repair creation failed. The DTO argument that was given is null." };

        // Check if the property with the specified PIN exists and if the OwnerVAT matches the property's owner
        var propertyItem = await db.Properties
            .Include(p => p.Owner)
            .FirstOrDefaultAsync(p => p.PIN == repairDto.PropertyIdNum && p.Owner.VATNum == repairDto.OwnerVAT);
        if (propertyItem == null)
            return new ResponseApi<RepairAdminCreateUpdateDTO> { Status = 1, Description = $"Either the property with PIN {repairDto.PropertyIdNum} does not exist or the owner with VAT {repairDto.OwnerVAT} is not authorized for this property." };

        // Validate the user input values
        var validationResponse = validation.RepairValidatorAdmin(repairDto);
        if (validationResponse != null)
        {
            return validationResponse;
        }

        // Map DTO to a new Repair entity (which is the one that ll be saved in the db, it having all the relationships, validations etc in contrast to the dto)
        var repair = new Repair
        {
            ScheduledDate = repairDto.ScheduledDate,
            RType = repairDto.RType,
            Description = repairDto.Description!,
            Status = repairDto.Status,
            Cost = repairDto.Cost,
            Property = propertyItem // Set PropertyId field value for the new repair db entry - EF will connect the dots
        };

        try
        {
            await db.Repairs.AddAsync(repair);
            await db.SaveChangesAsync();
            return new ResponseApi<RepairAdminCreateUpdateDTO> { Status = 0, Description = $"Repair for property with PIN {repairDto.PropertyIdNum} was created successfully.", Value = repair.ConvertRepairAdmin() };
        }
        catch (Exception e)
        {
            return new ResponseApi<RepairAdminCreateUpdateDTO> { Status = 1, Description = $"Repair creation for property with PIN {repairDto.PropertyIdNum} failed due to a database error: '{e.Message}'" };
        }
    }

    public async Task<ResponseApi<List<RepairDTO>>> GetAllPendingRepairs()
    {
        try
        {
            var GetQuery = await db.Repairs
                .Include(r => r.Property)
                .ThenInclude(r => r.Owner)
                .Where(x => x.Status == RepairStatus.Pending)
                .Select(x => x.ConvertRepair())
                .ToListAsync();

            string description = (GetQuery.Count == 0) ? "There are no pending repairs." : "List of pending repairs created.";
            int status = (GetQuery.Count == 0) ? 1 : 0;

            return new ResponseApi<List<RepairDTO>> { Value = GetQuery, Status = status, Description = description };
        }

        catch (Exception e)
        {
            return new ResponseApi<List<RepairDTO>> { Value = new(), Status = 0, Description = $"The list of pending repairs failed to create due to a database error: '{e.Message}'" };
        }
    }

    public async Task<ResponseApi<List<RepairDTO>>> GetAllOwnerRepairsByVAT(string? VATNum)
    {
        if (string.IsNullOrWhiteSpace(VATNum))
            return new ResponseApi<List<RepairDTO>> { Value = new(), Status = 1, Description = $"The VAT Number you entered is not valid. " };

        var GetQuery = await db.Repairs
            .Include(r => r.Property)
            .ThenInclude(r => r.Owner)
            .Where(r => r.Property.Owner.VATNum == VATNum)
            .Select(r => r.ConvertRepair())
            .ToListAsync();

        if (GetQuery.Count == 0)
        {
            return new ResponseApi<List<RepairDTO>> { Value = new(), Status = 1, Description = $"There are no repairs for owner with VAT Number: {VATNum} in the database. " };
        }

        try
        {
            return new ResponseApi<List<RepairDTO>> { Value = GetQuery, Status = 0, Description = $"List of repairs with VAT Number: {VATNum} created." };
        }
        catch (Exception e)
        {
            return new ResponseApi<List<RepairDTO>> { Value = new(), Status = 1, Description = $"The list of repairs with VAT Number: {VATNum} failed to create due to a database error: '{e.Message}'" };
        }
    }

    public async Task<ResponseApi<List<RepairDTO>>> SearchRepairs(int? id, DateOnly? startDate, DateOnly? endDate)
    {
        if (id == null && startDate == null)
            return new ResponseApi<List<RepairDTO>>
            {
                Status = 0,
                Description = "No search parameters.",
                Value = []
            };

        var results = db.Repairs
                .Include(a => a.Property)
                .ThenInclude(a => a.Owner)
                .AsQueryable();

        if (id is not null)
            results = results.Where(x => x.Property.Owner.Id == id);

        if (startDate != null)
        {
            if (endDate == null)
                results = results.Where(x => DateOnly.FromDateTime(x.ScheduledDate) == startDate);
            else
                results = results.Where(x => DateOnly.FromDateTime(x.ScheduledDate) >= startDate && DateOnly.FromDateTime(x.ScheduledDate) <= endDate);
        }

        var finalresults = await results
                    .Select(x => x.ConvertRepair())
                    .ToListAsync();

        try
        {
            return new ResponseApi<List<RepairDTO>> { Value = finalresults, Status = 0, Description = $"List of repeair search results created." };
        }
        catch (Exception e)
        {
            return new ResponseApi<List<RepairDTO>> { Status = 1, Description = $"The list of repairs with ID: {id} failed to create due to a database error: '{e.Message}'" };
        }
    }
    public async Task<ResponseApi<RepairDTO>> UpdateRepairUser(RepairDTO updatedRepairDto)
    {
        if (updatedRepairDto == null) return new ResponseApi<RepairDTO> { Status = 1, Description = $"The DTO argument that was given is null." };

        // Find the repair in the database
        var repairDb = await db.Repairs
        .Include(r => r.Property)
        .ThenInclude(p => p.Owner)
        .FirstOrDefaultAsync(r => r.Id == updatedRepairDto.Id);

        if (repairDb == null)
        {
            return new ResponseApi<RepairDTO> { Status = 1, Description = $"No repair with this ID was found in the database." };
        }

        var propertyId = await db.Properties
            .FirstOrDefaultAsync(p => p.PIN == updatedRepairDto.PropertyIdNum);
        if (propertyId == null)
        {
            return new ResponseApi<RepairDTO> { Status = 1, Description = $"No property with this PIN was found in the database." };
        }

        // Retrieve the list of property IDs associated with the owner of this repair
        var ownerId = repairDb.Property.Owner.Id;
        var ownerPINs = await db.Properties
            .Where(p => p.Owner.Id == ownerId)
            .Select(p => p.PIN)
            .ToListAsync();
        
        // Validate if the new PropertyId in updatedRepairDto exists within the owner's property list
        if (!ownerPINs.Contains(updatedRepairDto.PropertyIdNum!))
        {
            return new ResponseApi<RepairDTO> { Status = 1, Description = "The specified Property ID does not belong to the repair owner." };
        }

        // Validate the rest of the DTO's values
        var validationResponse = validation.RepairValidatorUser(updatedRepairDto);
        if (validationResponse != null)
        {
            return validationResponse;
        }

        // Retrieve the new Property entity based on updatedRepairDto.PropertyId
        var newProperty = await db.Properties.FirstOrDefaultAsync(p => p.PIN == updatedRepairDto.PropertyIdNum); ;
        if (newProperty == null)
        {
            return new ResponseApi<RepairDTO> { Status = 1, Description = $"No property with ID {updatedRepairDto.PropertyIdNum} was found in the database." };
        }

        //update with new values
        repairDb.Status = updatedRepairDto.Status;
        repairDb.ScheduledDate = updatedRepairDto.ScheduledDate;
        repairDb.RType = updatedRepairDto.RType;
        repairDb.Description = updatedRepairDto.Description!;
        repairDb.Cost = updatedRepairDto.Cost;
        repairDb.Property = newProperty; // Assign the new property object

        try
        {
            await db.SaveChangesAsync();
            // Return success response with the updated repair as RepairDTO
            return new ResponseApi<RepairDTO> { Status = 0, Description = $"Repair with ID {repairDb.Id} was updated successfully.", Value = repairDb.ConvertRepair() };
        }
        catch (Exception e)
        {
            return new ResponseApi<RepairDTO> { Status = 1, Description = $"Repair update for repair with ID {repairDb.Id} failed due to a database error: '{e.Message}'" };
        }
    }

    public async Task<ResponseApi<RepairAdminCreateUpdateDTO>> UpdateRepairAdmin(RepairAdminCreateUpdateDTO updatedRepairDto)
    {
        if (updatedRepairDto == null) return new ResponseApi<RepairAdminCreateUpdateDTO> { Status = 1, Description = $"The DTO argument that was given is null." };

        // Find the repair in the database
        var repairDb = await db.Repairs
        .Include(r => r.Property)
        .ThenInclude(p => p.Owner)
        .FirstOrDefaultAsync(r => r.Id == updatedRepairDto.Id);

        if (repairDb == null)
        {
            return new ResponseApi<RepairAdminCreateUpdateDTO> { Status = 1, Description = $"No repair with this ID was found in the database." };
        }

        // 1) Check if the property with the specified PIN exists and if the OwnerVAT matches the property's owner
        // 2) Retrieve the updated Property entity that will be connected to the repair, based on the updatedRepairDto.PropertyIdNum of the new DTO input
        var updatedProperty = await db.Properties
            .Include(p => p.Owner)
            .FirstOrDefaultAsync(p => p.PIN == updatedRepairDto.PropertyIdNum && p.Owner.VATNum == updatedRepairDto.OwnerVAT);
        if (updatedProperty == null)
            return new ResponseApi<RepairAdminCreateUpdateDTO> { Status = 1, Description = $"Either the property with PIN {updatedRepairDto.PropertyIdNum} does not exist or the owner with VAT {updatedRepairDto.OwnerVAT} is not authorized for this property." };

        // Retrieve the list of property IDs associated with the owner of this repair
        var ownerId = repairDb.Property.Owner.Id;
        var ownerPropertyIds = await db.Properties
            .Where(p => p.Owner.Id == ownerId)
            .Select(p => p.PIN)
            .ToListAsync();

        // Validate if the new PropertyId in updatedRepairDto exists within the owner's property list
        if (!ownerPropertyIds.Contains(updatedRepairDto.PropertyIdNum!))
        {
            return new ResponseApi<RepairAdminCreateUpdateDTO> { Status = 1, Description = "The specified Property ID does not belong to the repair owner." };
        }

        // Validate the rest of the DTO's values
        var validationResponse = validation.RepairValidatorAdmin(updatedRepairDto);
        if (validationResponse != null)
        {
            return validationResponse;
        }

        // Update the entity with the new DTO values
        repairDb.Status = updatedRepairDto.Status;
        repairDb.ScheduledDate = updatedRepairDto.ScheduledDate;
        repairDb.RType = updatedRepairDto.RType;
        repairDb.Description = updatedRepairDto.Description!;
        repairDb.Cost = updatedRepairDto.Cost;
        repairDb.Property = updatedProperty; // Assign the new property object

        try
        {
            await db.SaveChangesAsync();

            // Return success response with the updated repair as RepairDTO
            return new ResponseApi<RepairAdminCreateUpdateDTO> { Status = 0, Description = $"Repair with ID {repairDb.Id} was updated successfully.", Value = repairDb.ConvertRepairAdmin() };
        }
        catch (Exception e)
        {
            return new ResponseApi<RepairAdminCreateUpdateDTO> { Status = 1, Description = $"Repair update for repair with ID {repairDb.Id} failed due to a database error: '{e.Message}'" };
        }
    }

    public async Task<ResponseApi<RepairDeactivateResponseDTO>> DeactivateRepair(RepairDeactivateRequestDTO repairDTO)
    {
        // Check if the repair exists in the db
        var repair = await db.Repairs.FirstOrDefaultAsync(r => r.Id == repairDTO.RepairId);

        if (repair == null)
        {
            return new ResponseApi<RepairDeactivateResponseDTO> { Status = 1, Description = $"No repair with ID {repairDTO.RepairId} was found in the database." };
        }

        try
        {
            repair.IsActive = false;
            await db.SaveChangesAsync();

            // Create the response DTO with updated data
            var responseDTO = new RepairDeactivateResponseDTO
            {
                RepairId = repair.Id,
                IsActive = repair.IsActive
            };

            return new ResponseApi<RepairDeactivateResponseDTO> { Status = 0, Description = $"Repair with ID {repair.Id} was deactivated successfully.", Value = responseDTO };
        }
        catch (Exception ex)
        {
            return new ResponseApi<RepairDeactivateResponseDTO> { Status = 1, Description = $"Deactivation of repair with ID {repair.Id} failed due to a database error: {ex.Message}" };
        }
    }

    public async Task<ResponseApi<RepairDeactivateRequestDTO>> DeleteRepair(RepairDeactivateRequestDTO repairDTO)
    {
        // Check if the repair exists in the db
        var repair = await db.Repairs
            .Include(r => r.Property)
            .ThenInclude(p => p.Owner)
            .FirstOrDefaultAsync(r => r.Id == repairDTO.RepairId);

        if (repair == null)
        {
            return new ResponseApi<RepairDeactivateRequestDTO> { Status = 1, Description = $"No repair with ID equal to {repairDTO.RepairId} was found in the database." };
        }

        // Remove repair from the database (hard deletion)
        try
        {
            db.Repairs.Remove(repair);
            await db.SaveChangesAsync();

            return new ResponseApi<RepairDeactivateRequestDTO> { Status = 0, Description = $"Repair with ID {repair.Id} was deleted successfully.", Value = new RepairDeactivateRequestDTO { OwnerVAT = repair.Property.Owner.VATNum } };
        }
        catch (Exception ex)
        {
            return new ResponseApi<RepairDeactivateRequestDTO> { Status = 1, Description = $"Deletion of repair with ID {repair.Id} failed due to a database error: {ex.Message}" };
        }
    }
}
