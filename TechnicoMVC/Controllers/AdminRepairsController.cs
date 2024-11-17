using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Responses;

namespace TechnicoMVC.Controllers;

public class AdminRepairsController : Controller
{
    private readonly ILogger<RepairController> _logger;
    private readonly string sourcePrefix = "https://localhost:7017/api/Repair/"; //for other controller change to Repair / Property etc.
    private HttpClient client = new HttpClient();

    public AdminRepairsController(ILogger<RepairController> logger) => _logger = logger;


    public async Task<IActionResult> Index()
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

        return View("Error");
    }

    public async Task<IActionResult> Search([FromQuery] int? userId, [FromQuery]  DateOnly? startDate, [FromQuery] DateOnly? endDate)
    {
        string url = $"{sourcePrefix}repairs/search";

        var queryParams = new List<string>();
        if (userId != null) { queryParams.Add($"userId={userId}"); }
        if (startDate != null) { queryParams.Add($"startDate={startDate}"); }
        if (endDate != null) { queryParams.Add($"endDate={endDate}"); }
        var queryString = string.Join("&", queryParams);

        if (string.IsNullOrEmpty(queryString))
        {
            return View();
        }
        else { url = url + "?" + queryString; }

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

        return View("Error");
    }
}
