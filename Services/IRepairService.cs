using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Models;
using TechnicoBackEnd.ResponsesGT;

namespace TechnicoBackEnd.Services;
public interface IRepairService
{
    Task<ResponseApi<RepairDTO>> CreateRepair(RepairDTO repairDto, int propertyId);
    Task<ResponseApi<RepairDTO>> DeactivateRepair(int repairId);
    Task<ResponseApi<RepairDTO>> DeleteRepair(int repairId);
    Task<ResponseApi<List<RepairDTO>>> GetAllOwnerRepairsByDateOrRangeOfDates(DateTime StartDate, DateTime EndDate);
    Task<ResponseApi<List<RepairDTO>>> GetAllOwnerRepairsByVAT(string? VATNum);
    Task<ResponseApi<List<RepairDTO>>> GetAllPendingRepairs(RepairStatus Status);
    Task<ResponseApi<RepairDTO>> GetRepairByID(int Id);
    Task<ResponseApi<RepairDTO>> UpdateRepair(RepairDTO updatedRepairDto);
}