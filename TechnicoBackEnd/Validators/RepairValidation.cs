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
        if (repair != null) { 
            // 1. Validate repair status
            if (repair.Status != RepairStatus.Pending)
            {
                return new ResponseApi<RepairDTO> { Status = 1, Description = $"Invalid status value: {repair.Status}. Repair status on user creation must always be Pending." };
            }

            // 2a. Validate ScheduledDate
            if (repair.ScheduledDate == default)
            {
                return new ResponseApi<RepairDTO> { Status = 1, Description = "ScheduledDate is required and cannot be the default date." };
            }

            // 2b. Validate ScheduledDate
            if (repair.ScheduledDate < DateTime.Now.AddHours(1))
            {
                return new ResponseApi<RepairDTO> { Status = 1, Description = "ScheduledDate must be at least one hour in the future." };
            }

            // 3. Validate RepairType
            if (!Enum.IsDefined(typeof(RepairType), repair.RType))
            {
                return new ResponseApi<RepairDTO> { Status = 1, Description = $"Invalid repair type value: {repair.RType}. Please provide a valid repair type (e.g., Painting, Insulation, Frames, Plumbing, Electrical)." };
            }

            // 4. Validate Description
            if (string.IsNullOrWhiteSpace(repair.Description))
            {
                return new ResponseApi<RepairDTO> { Status = 1, Description = "Description cannot be empty or whitespace." };
            }

            // 5. Validate Cost
            if (repair.Cost <= 0)
            {
                return new ResponseApi<RepairDTO> { Status = 1, Description = "Cost must be greater than zero." };
            }

            // If everything is valid, return null (indicating no errors)
            return null;
        }
        return null;
    }

    public ResponseApi<RepairAdminCreateUpdateDTO>? RepairValidatorAdmin(RepairAdminCreateUpdateDTO repair)
    {
        if (repair != null)
        {
            // 1. Validate repair status
            if (!Enum.IsDefined(typeof(RepairStatus), repair.Status))
            {
                return new ResponseApi<RepairAdminCreateUpdateDTO> { Status = 1, Description = $"Invalid status value: {repair.Status}. Please provide a valid status (e.g., Pending, InProgress, Complete)." };
            }

            // 2a. Validate ScheduledDate
            if (repair.ScheduledDate == default)
            {
                return new ResponseApi<RepairAdminCreateUpdateDTO> { Status = 1, Description = "ScheduledDate is required and cannot be the default date." };
            }

            // 2b. Validate ScheduledDate
            if (repair.ScheduledDate < DateTime.Now.AddHours(1))
            {
                return new ResponseApi<RepairAdminCreateUpdateDTO> { Status = 1, Description = "ScheduledDate must be at least one hour in the future." };
            }

            // 3. Validate RepairType
            if (!Enum.IsDefined(typeof(RepairType), repair.RType))
            {
                return new ResponseApi<RepairAdminCreateUpdateDTO> { Status = 1, Description = $"Invalid repair type value: {repair.RType}. Please provide a valid repair type (e.g., Painting, Insulation, Frames, Plumbing, Electrical)." };
            }

            // 4. Validate Description
            if (string.IsNullOrWhiteSpace(repair.Description))
            {
                return new ResponseApi<RepairAdminCreateUpdateDTO> { Status = 1, Description = "Description cannot be empty or whitespace." };
            }

            // 5. Validate Cost
            if (repair.Cost <= 0)
            {
                return new ResponseApi<RepairAdminCreateUpdateDTO> { Status = 1, Description = "Cost must be greater than zero." };
            }

            // If everything is valid, return null (indicating no errors)
            return null;
        }
        return null;
    }
}
