using Microsoft.AspNetCore.Mvc;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Responses;

namespace TechnicoMVC.Controllers;
public class RepairController : Controller
{

    private readonly ILogger<RepairController> _logger;
    private readonly string sourcePrefix = "https://localhost:7017/api/Repair/"; //for other controller change to Repair / Property etc.
    private HttpClient client = new HttpClient();

    public RepairController(ILogger<RepairController> logger) => _logger = logger;


    public async Task<IActionResult> UserRepairs() //https://localhost:7130/Repair/UserRepairs για να το δω
    {
        // Define the API endpoint for retrieving repairs
        string url = $"{sourcePrefix}repairs/get_all_pending";
        var response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // Deserialize the response body to ResponseApi<List<RepairDTO>>
            var apiResponse = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<List<RepairDTO>>>(responseBody, options);

            if (apiResponse?.Value != null)
            {
                return View(apiResponse.Value);
            }
        }

        // If the API call fails or data is null, return an empty list
        return View(new List<RepairDTO>());
    }

    
}
