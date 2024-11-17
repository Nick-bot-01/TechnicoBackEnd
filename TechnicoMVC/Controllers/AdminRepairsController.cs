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


    public async Task<IActionResult> GetAllUsersRepairs()
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

    public async Task<ResponseApi<RepairDeactivateRequestDTO>?> RemoveRepairToRedirectController(int repairId)
    {
        string url = $"{sourcePrefix}repair/delete";

        // Create the DTO object with the RepairId
        var repairDTO = new RepairDeactivateRequestDTO { RepairId = repairId };

        // Create the DELETE request with the DTO in the body
        var request = new HttpRequestMessage(HttpMethod.Delete, url)
        {
            Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(repairDTO), System.Text.Encoding.UTF8, "application/json")
        };

        var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();

            ResponseApi<RepairDeactivateRequestDTO>? removedRepair = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<RepairDeactivateRequestDTO>>(responseBody);
            return removedRepair;
        }

        // Log error details if the request fails
        var errorResponse = await response.Content.ReadAsStringAsync();
        return null;
    }

    [HttpPost]
    public async Task<IActionResult> RemoveRepairCallback(int repairId)
    {
        if (repairId <= 0)
        {
            return RedirectToAction("GetAllUsersRepairs");
        }

        var deletedRepair = await RemoveRepairToRedirectController(repairId);

        if (deletedRepair != null)
        {
            _logger.LogInformation("Successfully deleted repair with ID: {RepairId}", repairId);
        }
        else
        {
            _logger.LogError("Deletion failed for repair ID: {RepairId}", repairId);
            return View("Error");
        }

        return RedirectToAction("GetAllUsersRepairs", new { VATNum = deletedRepair?.Value?.OwnerVAT });
    }


    // CREATE

    [HttpPost]
    public async Task<ResponseApi<RepairDTO>?> CreateRepairToRedirectController(RepairDTO repair)
    {
        string url = $"{sourcePrefix}admin/create_repair";
        var response = await client.PostAsJsonAsync(url, repair);
        var responseBody = await response.Content.ReadAsStringAsync();
        ResponseApi<RepairDTO>? targetRepair = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<RepairDTO>>(responseBody);
        return targetRepair;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRepairCallback(RepairDTO pendingCreationRepair)
    {
        ResponseApi<RepairDTO>? createdRepair = await CreateRepairToRedirectController(pendingCreationRepair);

        if (createdRepair?.Value != null)
        {
            if (createdRepair?.Status == 0)
            {
                pendingCreationRepair.ErrorCode = 0;
                pendingCreationRepair.ErrorDescription = createdRepair.Description;
            }
            return View("AdminCreateRepair", pendingCreationRepair);
        }
        else
        {
            if (createdRepair?.Status == 1)
            {
                pendingCreationRepair.ErrorCode = 1;
                pendingCreationRepair.ErrorDescription = createdRepair.Description;
            }
            return View("AdminCreateRepair", pendingCreationRepair);
        }
    }

    public IActionResult AdminCreateRepair()
    {
        return View(new RepairDTO { ScheduledDate = DateTime.Now, PropertyIdNum = "", Cost = 0, Description = "" });
    }


    // UPDATE

    [HttpGet]
    public async Task<ResponseApi<RepairDTO>?> GetUpdatePageRedirect(int id)
    {
        string url = $"{sourcePrefix}repairs/get_repair_details/{id}";
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        ResponseApi<RepairDTO>? targetRepair = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<RepairDTO>>(responseBody);
        return targetRepair;
    }

    public async Task<IActionResult> GetUpdatePageCallback(RepairDTO pendingRepairDetails)
    {
        ResponseApi<RepairDTO>? repairDetails = await GetUpdatePageRedirect(pendingRepairDetails.Id);

        return View("AdminUpdateRepair", repairDetails?.Value);
    }


    [HttpPut]
    public async Task<ResponseApi<RepairDTO>?> UpdateAdminToRedirectController(RepairDTO repair)
    {
        string url = $"{sourcePrefix}admin/update_repair";
        var response = await client.PutAsJsonAsync(url, repair);
        var responseBody = await response.Content.ReadAsStringAsync();
        ResponseApi<RepairDTO>? targetRepair = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<RepairDTO>>(responseBody);
        return targetRepair;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRepairCallback(RepairDTO pendingUpdateRepair)
    {
        ResponseApi<RepairDTO>? createdRepair = await UpdateAdminToRedirectController(pendingUpdateRepair);

        if (createdRepair?.Value != null)
        {
            if (createdRepair?.Status == 0)
            {
                pendingUpdateRepair.ErrorCode = 0;
                pendingUpdateRepair.ErrorDescription = createdRepair.Description;
            }
            return View("AdminUpdateRepair", pendingUpdateRepair);
        }
        else
        {
            if (createdRepair?.Status == 1)
            {
                pendingUpdateRepair.ErrorCode = 1;
                pendingUpdateRepair.ErrorDescription = createdRepair.Description;
            }
            return View("AdminUpdateRepair", pendingUpdateRepair);
        }
    }

    public IActionResult AdminUpdateRepair()
    {
        return View();
    }


    public async Task<IActionResult> Search([FromQuery] int? userId, [FromQuery] DateOnly? startDate, [FromQuery] DateOnly? endDate)
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





