using Microsoft.AspNetCore.Mvc;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Models;
using TechnicoBackEnd.Services;
using TechnicoBackEnd.Helpers;
using TechnicoBackEnd.Responses;


namespace TechnicoWebAPI.Controllers;



[Route("api/[controller]")]
[ApiController]
public class RepairController : ControllerBase
{
    private readonly IRepairService _service;


    public RepairController(IRepairService repairService)
    {
        _service = repairService;
    }


    [HttpGet("Repairs,Route({RepairStatus})")]
    public async Task<ActionResult<List<RepairDTO>>> GetAllPendingRepairs([FromRoute] RepairStatus status)
    {

        var response = await _service.GetAllPendingRepairs(status);
        if (response == null) { return NotFound(); }

        return Ok(response);
    }

    [HttpGet("Repairs by VAT,Route({VATNum})")]
    public async Task<ActionResult<List<RepairDTO>>> GetAllOwnerRepairsByVAT([FromRoute] string? VATNum)
    {
        var response = await _service.GetAllOwnerRepairsByVAT(VATNum);
        if (response == null) { return NotFound(); }

        return Ok(response);
    }

    [HttpGet("Repairs by Date,Route({StartDate,EndDate})")]

    public async Task<ActionResult<List<RepairDTO>>> GetAllOwnerRepairsByDateOrRangeOfDates([FromRoute]DateTime StartDate,[FromRoute]DateTime EndDate)
    {
        var response = await _service.GetAllOwnerRepairsByDateOrRangeOfDates(StartDate, EndDate);
        if (response == null) { return NotFound(); }

        return Ok(response);
    }

    [HttpGet("Repairs by ID,Route({id})")]

    public async Task<ActionResult<RepairDTO>> GetRepairByID([FromRoute]int Id) 
    {
        var response = await _service.GetRepairByID(Id);
        if (response == null) { return NotFound(); }

        return Ok(response);

     }

}