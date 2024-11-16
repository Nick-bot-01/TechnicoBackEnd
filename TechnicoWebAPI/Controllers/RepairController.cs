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

    [HttpPost("user/create_repair")]
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
    public async Task<ActionResult<List<RepairDTO>>> GetAllPendingRepairs()
    {

        var response = await _repairService.GetAllPendingRepairs();
        if (response == null) { return NotFound(); }

        return Ok(response);
    }

    [HttpGet("repairs/get_all_by_vat/{VATNum}")]
    public async Task<ActionResult<List<RepairDTO>>> GetAllOwnerRepairsByVAT([FromRoute] string? VATNum)
    {
        var response = await _repairService.GetAllOwnerRepairsByVAT(VATNum);
        if (response == null) { return NotFound(); }

        return Ok(response);
    }

    [HttpGet("repairs/get_all_by_id/{id}")]
    public async Task<ActionResult<List<RepairDTO>>> GetAllOwnerRepairsByUID([FromRoute] int id)
    {
        var response = await _repairService.GetAllOwnerRepairsByUID(id);
        if (response == null) { return NotFound(); }

        return Ok(response);
    }

    [HttpGet("repairs/get_all_by_dates")]
    public async Task<ActionResult<List<RepairDTO>>> GetAllOwnerRepairsByDateOrRangeOfDates(DateTime StartDate, DateTime EndDate)
    {
        var response = await _repairService.GetAllOwnerRepairsByDateOrRangeOfDates(StartDate, EndDate);
        if (response == null) { return NotFound(); }

        return Ok(response);
    }

    [HttpGet("repairs/get_by_repair_id/{id}")]
    public async Task<ActionResult<RepairDTO>> GetRepairByID([FromRoute] int id)
    {
        var response = await _repairService.GetRepairByID(id);
        if (response == null) { return NotFound(); }

        return Ok(response);

    }
}
