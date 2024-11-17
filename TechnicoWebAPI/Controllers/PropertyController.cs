using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Responses;
using TechnicoBackEnd.Services;

namespace TechnicoWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PropertyController : ControllerBase
{
    private readonly IPropertyService _propertyService;

    public PropertyController(IPropertyService propertyService) => _propertyService = propertyService;

    [HttpGet("properties")]
    public async Task<ResponseApi<List<PropertyDTO>>> GetAllProperties() => await _propertyService.GetAllProperties();

    [HttpGet("properties/id/{id}")]
    public async Task<ResponseApi<PropertyDTO>> GetPropertyById([FromRoute] int id) => await _propertyService.GetPropertyById(id);

    [HttpGet("properties/vat/{vat}")]
    public async Task<ResponseApi<List<PropertyDTO>>> GetPropertiesByOwner([FromRoute] string vat) => await _propertyService.GetPropertiesByOwner(vat);

    [HttpGet("properties/byid/{id}")]
    public async Task<ResponseApi<List<PropertyDTO>>> GetPropertiesByOwnerID([FromRoute] int id) => await _propertyService.GetPropertiesByOwnerID(id);

    [HttpGet("search_properties")]
    public async Task<ResponseApi<List<PropertyDTO>>> SearchProperty([FromQuery] string? pin, [FromQuery] string? vat) => await _propertyService.SearchProperties(pin,vat);

    [HttpPost("create_property")]
    public async Task<ResponseApi<PropertyDTO>> CreateProperty([FromBody] PropertyDTO property) => await _propertyService.CreateProperty(property);
    [HttpPatch("update_property")]
    public async Task<ResponseApi<PropertyDTO>> UpdateProperty([FromBody] PropertyDTO property) => await _propertyService.UpdateProperty(property);

    [HttpDelete("delete/{id}")]
    public async Task<ResponseApi<PropertyDTO>> DeleteProperty([FromRoute] int id) => await _propertyService.DeleteProperty(id);

    [HttpDelete("deactivate/{id}")]
    public async Task<ResponseApi<PropertyDTO>> DeactivateProperty([FromRoute] int  id) => await _propertyService.DeactivateProperty(id);
}
