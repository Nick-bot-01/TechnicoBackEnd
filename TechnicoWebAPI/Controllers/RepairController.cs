using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Models;
using TechnicoBackEnd.Responses;
using TechnicoBackEnd.Services;

namespace TechnicoWebAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class RepairController : ControllerBase
{
    private readonly IRepairService _repairService;

    public RepairController(IRepairService repairService)
    {
        _repairService = repairService;
    }

    [HttpPost("repair/user")]
    public async Task<ResponseApi<RepairDTO>> CreateRepairUser([FromBody] RepairDTO repairDTO)
    {
        return await _repairService.CreateRepairUser(repairDTO);
    }

    [HttpPost("repair/admin")]
    public async Task<ResponseApi<RepairAdminCreateUpdateDTO>> CreateRepairAdmin([FromBody] RepairAdminCreateUpdateDTO repairDTO)
    {
        return await _repairService.CreateRepairAdmin(repairDTO);
    }

    [HttpPut("repair/user")]
    public async Task<ResponseApi<RepairDTO>> UpdateRepairUser([FromBody] RepairDTO repairDTO)
    {
        return await _repairService.UpdateRepairUser(repairDTO);
    }

    [HttpPut("repair/admin")]
    public async Task<ResponseApi<RepairAdminCreateUpdateDTO>> UpdateRepairAdmin([FromBody] RepairAdminCreateUpdateDTO repairDTO)
    {
        return await _repairService.UpdateRepairAdmin(repairDTO);
    }

    [HttpDelete("repair/deactivate")]
    public async Task<ResponseApi<RepairDeactivateResponseDTO>> DeactivateRepair([FromBody] RepairDeactivateRequestDTO repairDTO)
    {
        return await _repairService.DeactivateRepair(repairDTO);
    }

    [HttpDelete("repair/delete")]
    public async Task<ResponseApi<RepairDeactivateRequestDTO>> DeleteRepair([FromBody] RepairDeactivateRequestDTO repairDTO)
    {
        return await _repairService.DeleteRepair(repairDTO);
    }

    [HttpGet("repairs/get_all_pending")]
    public async Task<ResponseApi<List<RepairDTO>>> GetAllPendingRepairs()
    {
        var response = await _repairService.GetAllPendingRepairs();
        return response;
    }

    [HttpGet("repairs/get_all_by_vat/{VATNum}")]
    public async Task<ResponseApi<List<RepairDTO>>> GetAllOwnerRepairsByVAT([FromRoute] string? VATNum){
        var response = await _repairService.GetAllOwnerRepairsByVAT(VATNum);
        return response;
    }

    [HttpGet("repairs/get_all_by_dates")]
    public async Task<ResponseApi<List<RepairDTO>>> GetAllOwnerRepairsByDateOrRangeOfDates(DateTime StartDate, DateTime EndDate)
    {
        var response = await _repairService.GetAllOwnerRepairsByDateOrRangeOfDates(StartDate, EndDate);
        return response;
    }

    [HttpGet("repairs/get_all_daily")]
    public async Task<ResponseApi<List<RepairDTO>>> GetAllRepairsDaily()
    {
        var response = await _repairService.GetAllOwnerRepairsByDateOrRangeOfDates(DateTime.Today, DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59));
        return response;
    }

    [HttpGet("repairs/get_by_repair_id/{id}")]
    public async Task<ResponseApi<RepairDTO>> GetRepairByID([FromRoute] int id)
    {
        var response = await _repairService.GetRepairByID(id);
        return response;

    }
}
