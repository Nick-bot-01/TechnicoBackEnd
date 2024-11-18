using Microsoft.AspNetCore.Mvc;
using TechnicoBackEnd.Auth;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Responses;

namespace TechnicoMVC.Controllers;
public class UserPropertiesController : Controller
{
    private readonly ILogger<UserPropertiesController> _logger;
    private readonly string sourcePrefix = "https://localhost:7017/api/Property/";
    private HttpClient client = new HttpClient();

    public UserPropertiesController(ILogger<UserPropertiesController> logger) => _logger = logger;

    public async Task<IActionResult> GetUserPropertiesByUID(int id)
    {
        if (id <= 0)
        {
            return View("Error", "Negative user Id Detected");
        }
        // Define the API endpoint for retrieving repairs
        string url = $"{sourcePrefix}properties/byid/{id}";
        var response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // Deserialize the response body to ResponseApi<List<PropertyDTO>>
            var apiResponse = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<List<PropertyDTO>>>(responseBody, options);

            if (apiResponse?.Value != null)
            {
                return View("GetUserProperties", apiResponse.Value);
            }
        }

        return View("Error");
    }

    //   DELETE

    public async Task<ResponseApi<PropertyDTO>?> DeletePropertyToRedirectController(int propertyId)
    {
        string url = $"{sourcePrefix}delete/{propertyId}";

        // Create the DTO object with the RepairId
        var repairDTO = new PropertyDTO { Id = propertyId };

        // Create the DELETE request with the DTO in the body
        var request = new HttpRequestMessage(HttpMethod.Delete, url)
        {
            Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(repairDTO), System.Text.Encoding.UTF8, "application/json")
        };

        var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();

            ResponseApi<PropertyDTO>? removedProperty = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<PropertyDTO>>(responseBody);
            return removedProperty;
        }

        // Log error details if the request fails
        var errorResponse = await response.Content.ReadAsStringAsync();
        return null;
    }

    [HttpPost]
    public async Task<IActionResult> DeletePropertyCallback(int propertyId)
    {
        if (propertyId <= 0)
        {
            return RedirectToAction("GetUserProperties");
        }

        var deletedProperty = await DeletePropertyToRedirectController(propertyId);

        if (deletedProperty != null)
        {
            _logger.LogInformation("Successfully deleted repair with ID: {RepairId}", propertyId);
        }
        else
        {
            _logger.LogError("Deletion failed for repair ID: {RepairId}", propertyId);
            return View("Error");
        }

        return RedirectToAction("GetUserPropertiesByUID", new { id = LoginState.UserId});
    }



    

    //deactivate/{id}



}
