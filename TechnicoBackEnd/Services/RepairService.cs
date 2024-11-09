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

    public async Task<ResponseApi<RepairDTO>> CreateRepair(RepairDTO repairDto, int propertyId)
    {
        // Check if the property intended for repair exists in the db
        var propertyItem = await db.Properties.FirstOrDefaultAsync(p => p.Id == propertyId);
        if (propertyItem == null)
            return new ResponseApi<RepairDTO> { Status = 1, Description = $"Repair creation failed. Property with id {propertyId} was not found." };

        // Validate the user input values
        var validationResponse = validation.RepairValidator(repairDto);
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
            return new ResponseApi<RepairDTO> { Status = 0, Description = $"Repair for property with id {propertyId} was created successfully.", Value = repair.ConvertRepair() };
        }
        catch (Exception e)
        {
            return new ResponseApi<RepairDTO> { Status = 1, Description = $"Repair creation for property with id {propertyId} failed due to a database error: '{e.Message}'" };
        }
    }

    public async Task<ResponseApi<List<RepairDTO>>> GetAllPendingRepairs(RepairStatus Status)
    {
        try
        {
            var GetQuery = await db.Repairs.Where(x => x.Status == Status)
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
            return new ResponseApi<List<RepairDTO>> { Status = 1, Description = $"The VAT Number you entered is not valid. " };

        var GetQuery = await db.Repairs.Where(x => x.Owner.VATNum == VATNum)
            .Select(x => x.ConvertRepair())
            .ToListAsync();

        if (GetQuery.Count == 0)
        {
            return new ResponseApi<List<RepairDTO>> { Status = 1, Description = $"There are no repairs with VAT Number: {VATNum} found in the database. " };
        }

        try
        {
            return new ResponseApi<List<RepairDTO>> { Value = GetQuery, Status = 0, Description = $"List of repairs with VAT Number: {VATNum} created." };
        }
        catch (Exception e)
        {
            return new ResponseApi<List<RepairDTO>> { Status = 1, Description = $"The list of repairs with VAT Number: {VATNum} failed to create due to a database error: '{e.Message}'" };
        }
    }

    public async Task<ResponseApi<List<RepairDTO>>> GetAllOwnerRepairsByDateOrRangeOfDates(DateTime StartDate, DateTime EndDate)
    {
        if (EndDate < StartDate)
        {
            return new ResponseApi<List<RepairDTO>> { Status = 1, Description = $"Please enter valid Dates." };
        }
        if (EndDate == DateTime.MinValue)
        {
            var GetQuery = await db.Repairs.Where(x => x.ScheduledDate == StartDate)
                .Select(x => x.ConvertRepair())
                .ToListAsync();

            try
            {

                return new ResponseApi<List<RepairDTO>> { Value = GetQuery, Status = 0, Description = $"Repair with date:{StartDate} created." };
            }
            catch (Exception e)
            {
                return new ResponseApi<List<RepairDTO>> { Status = 1, Description = $"Repair with date:{StartDate} failed to create due to a database error: '{e.Message}'" };
            }
        }

        var GetQuery2 = await db.Repairs.Where(x => x.ScheduledDate >= StartDate && x.ScheduledDate <= EndDate)
            .Select(x => x.ConvertRepair())
            .ToListAsync();
        try
        {
            return new ResponseApi<List<RepairDTO>> { Value = GetQuery2, Description = $"List with dates between {StartDate} and {EndDate} created." };
        }
        catch (Exception e)
        {
            return new ResponseApi<List<RepairDTO>> { Status = 1, Description = $"List with dates between {StartDate} and {EndDate} failed to create due to a database error: '{e.Message}'" };
        }
    }

    public async Task<ResponseApi<RepairDTO>> GetRepairByID(int Id)
    {
        var GetQuery = await db.Repairs.Where(x => x.Id == Id)
            .Select(x => x.ConvertRepair())
            .FirstOrDefaultAsync();

        if (GetQuery == null)
        {
            return new ResponseApi<RepairDTO> { Status = 1, Description = $"Repair with id: {Id} was not found." };
        }

        return new ResponseApi<RepairDTO> { Value = GetQuery, Description = $"Repair with id: {Id} was found." };
    }

    public async Task<ResponseApi<RepairDTO>> UpdateRepair(RepairDTO updatedRepairDto)
    {
        // Find the repair in the database
        var repairDb = await db.Repairs.FirstOrDefaultAsync(r => r.Id == updatedRepairDto.Id);

        if (repairDb == null)
        {
            return new ResponseApi<RepairDTO> { Status = 1, Description = $"No repair with this ID was found in the database." };
        }

        // Validate the DTO
        var validationResponse = RepairValidation.RepairValidator(updatedRepairDto);
        if (validationResponse != null)
        {
            return validationResponse;
        }

        //update with new values
        repairDb.Status = updatedRepairDto.Status;
        repairDb.ScheduledDate = updatedRepairDto.ScheduledDate;
        repairDb.RType = updatedRepairDto.RType;
        repairDb.Description = updatedRepairDto.Description!;
        repairDb.Cost = updatedRepairDto.Cost;

        // Update fields only if their value has changed - with ternary
        //repairDb.Status = (repairDb.Status != updatedRepairDto.Status) ? updatedRepairDto.Status : repairDb.Status;
        //repairDb.ScheduledDate = (repairDb.ScheduledDate != updatedRepairDto.ScheduledDate) ? updatedRepairDto.ScheduledDate : repairDb.ScheduledDate;
        //repairDb.RType = (repairDb.RType != updatedRepairDto.RType) ? updatedRepairDto.RType : repairDb.RType;
        //repairDb.Description = (repairDb.Description != updatedRepairDto.Description) ? updatedRepairDto.Description! : repairDb.Description;
        //repairDb.Cost = (repairDb.Cost != updatedRepairDto.Cost) ? updatedRepairDto.Cost : repairDb.Cost;

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

    public async Task<ResponseApi<RepairDTO>> DeactivateRepair(int repairId)
    {
        // Check if the repair exists in the db
        var repair = await db.Repairs.FirstOrDefaultAsync(r => r.Id == repairId);

        if (repair == null)
        {
            return new ResponseApi<RepairDTO> { Status = 1, Description = $"No repair with this ID was found in the database." };
        }

        // Repair becomes inactive (soft deletion)
        repair.IsActive = false;
        await db.SaveChangesAsync();

        return new ResponseApi<RepairDTO>
        { Status = 0, Description = $"Repair with ID {repair.Id} was deactivated successfully.", Value = repair.ConvertRepair() };
    }

    public async Task<ResponseApi<RepairDTO>> DeleteRepair(int repairId)
    {
        // Check if the repair exists in the db
        var repair = await db.Repairs.FirstOrDefaultAsync(r => r.Id == repairId);

        if (repair == null)
        {
            return new ResponseApi<RepairDTO> { Status = 1, Description = $"No repair with this ID was found in the database." };
        }

        // Remove repair from the database (hard deletion)
        db.Repairs.Remove(repair);
        await db.SaveChangesAsync();

        return new ResponseApi<RepairDTO>
        { Status = 0, Description = $"Repair with ID {repair.Id} was deleted successfully.", Value = repair.ConvertRepair() };
    }
}
