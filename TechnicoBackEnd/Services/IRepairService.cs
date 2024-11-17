using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Models;
using TechnicoBackEnd.Responses;

namespace TechnicoBackEnd.Services;
public interface IRepairService
{
    Task<ResponseApi<RepairDTO>> CreateRepairUser(RepairDTO repairDto);
    Task<ResponseApi<RepairAdminCreateUpdateDTO>> CreateRepairAdmin(RepairAdminCreateUpdateDTO repairDto);
    Task<ResponseApi<List<RepairDTO>>> SearchRepairs(int? userId, DateOnly? StartDate, DateOnly? EndDate);
    Task<ResponseApi<List<RepairDTO>>> GetAllOwnerRepairsByDateOrRangeOfDates(DateTime StartDate, DateTime EndDate);
    Task<ResponseApi<List<RepairDTO>>> GetAllOwnerRepairsByVAT(string? VATNum);
    Task<ResponseApi<List<RepairDTO>>> GetAllOwnerRepairsByUID(int id);
    Task<ResponseApi<List<RepairDTO>>> GetAllPendingRepairs();
    Task<ResponseApi<RepairDTO>> GetRepairByID(int Id);
    Task<ResponseApi<RepairDTO>> UpdateRepairUser(RepairDTO updatedRepairDto);
    Task<ResponseApi<RepairAdminCreateUpdateDTO>> UpdateRepairAdmin(RepairAdminCreateUpdateDTO updatedRepairDto);
    Task<ResponseApi<RepairDeactivateResponseDTO>> DeactivateRepair(RepairDeactivateRequestDTO repairDTO);
    Task<ResponseApi<RepairDeactivateRequestDTO>> DeleteRepair(RepairDeactivateRequestDTO repairDTO);
}