using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Models;
using TechnicoBackEnd.Responses;
using TechnicoBackEnd.Services;

namespace TechnicoWebAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class Repair2Controller : ControllerBase
{
    private readonly IRepairService _repairService;
    //private readonly 

    public Repair2Controller(IRepairService repairService)
    {
        _repairService = repairService;
    }

    //[HttpGet("repairs")]
    //public Task<ResponseApi<List<RepairDTO>>> GetRepairs()
    //{
    //    return _repairService.GetAllPendingRepairs(RepairStatus.Pending);
    //}

    [HttpPost("repair")]
    public async Task<ActionResult<RepairDTO>> Post([FromBody] RepairDTO repairDTO)
    {
        try
        {
            var response = await _repairService.CreateRepair(repairDTO, 2);
            return Ok(response);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

}
