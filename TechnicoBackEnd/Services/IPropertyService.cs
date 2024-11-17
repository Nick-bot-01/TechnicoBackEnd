using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Responses;

public interface IPropertyService
{
    Task<ResponseApi<PropertyDTO>> CreateProperty(PropertyDTO property);
    Task<ResponseApi<PropertyDTO>> DeactivateProperty(int id);
    Task<ResponseApi<PropertyDTO>> DeleteProperty(int id);
    Task<ResponseApi<List<PropertyDTO>>> GetAllProperties();
    Task<ResponseApi<List<PropertyDTO>>> GetPropertiesByOwner(string vat);
    Task<ResponseApi<List<PropertyDTO>>> GetPropertiesByOwnerID(int id);
    Task<ResponseApi<PropertyDTO>> GetPropertyById(int id);
    Task<ResponseApi<List<PropertyDTO>>> SearchProperties(string? pin = null, string? vat = null);
    Task<ResponseApi<PropertyDTO>> UpdateProperty(PropertyDTO property);
}