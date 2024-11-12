using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Models;
using TechnicoBackEnd.Responses;

namespace TechnicoBackEnd.Validators;
public class RepairValidation : IRepairValidation
{
    public ResponseApi<RepairDTO>? RepairValidatorUser(RepairDTO repair)
    {
        // Validate that repair is not null and status is Pending (specifically for the user)
        if (repair == null) return null;

        if (repair.Status != RepairStatus.Pending)
        {
            return new ResponseApi<RepairDTO>
            {
                Status = 1,
                Description = $"Invalid status value: {repair.Status}. Repair status on user creation must always be Pending."
            };
        }

        // Call the common fields validator method - we use ! for Description nullability warning since we do check if it's null inside the validator
        return ValidateCommonFields<RepairDTO>(repair.ScheduledDate, repair.RType, repair.Description!, repair.Cost);
    }

    public ResponseApi<RepairAdminCreateUpdateDTO>? RepairValidatorAdmin(RepairAdminCreateUpdateDTO repair)
    {
        // Validate that repair is not null and status is valid for admin (any choice within the enum's choices for admin)
        if (repair == null) return null;

        if (!Enum.IsDefined(typeof(RepairStatus), repair.Status))
        {
            return new ResponseApi<RepairAdminCreateUpdateDTO>
            {
                Status = 1,
                Description = $"Invalid status value: {repair.Status}. Please provide a valid status (e.g., Pending, InProgress, Complete)."
            };
        }

        // Call the common fields validator method - we use ! for Description nullability warning since we do check if it's null inside the validator
        return ValidateCommonFields<RepairAdminCreateUpdateDTO>(repair.ScheduledDate, repair.RType, repair.Description!, repair.Cost);
    }

    // Common fields validator - using <T> (Generics) so it can work for both RepairDTO and RepairAdminCreateUpdateDTO
    private ResponseApi<T>? ValidateCommonFields<T>(DateTime scheduledDate, RepairType rType, string description, decimal cost)
    {
        // Validate ScheduledDate
        if (scheduledDate == default)
        {
            return new ResponseApi<T> { Status = 1, Description = "ScheduledDate is required and cannot be the default date." };
        }
        if (scheduledDate < DateTime.Now.AddHours(1))
        {
            return new ResponseApi<T> { Status = 1, Description = "ScheduledDate must be at least one hour in the future." };
        }

        // Validate RepairType
        if (!Enum.IsDefined(typeof(RepairType), rType))
        {
            return new ResponseApi<T> { Status = 1, Description = $"Invalid repair type value: {rType}. Please provide a valid repair type (e.g., Painting, Insulation, Frames, Plumbing, Electrical)." };
        }

        // Validate Description
        if (string.IsNullOrWhiteSpace(description))
        {
            return new ResponseApi<T> { Status = 1, Description = "Description cannot be empty or whitespace." };
        }

        // Validate Cost
        if (cost <= 0)
        {
            return new ResponseApi<T> { Status = 1, Description = "Cost must be greater than zero." };
        }

        // If everything is valid, return null
        return null;
    }
}
