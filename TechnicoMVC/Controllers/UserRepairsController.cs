using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using TechnicoBackEnd.Auth;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Responses;

namespace TechnicoMVC.Controllers;

public class UserRepairsController : Controller
{
    private readonly ILogger<RepairController> _logger;
    private readonly string sourcePrefix = "https://localhost:7017/api/Repair/"; //for other controller change to Repair / Property etc.
    private HttpClient client = new HttpClient();

    public UserRepairsController(ILogger<RepairController> logger) => _logger = logger;


    public async Task<IActionResult> GetUserRepairs(string VATNum)
    {
        if (string.IsNullOrWhiteSpace(VATNum))
        {
            return View("Error");
        }
        // Define the API endpoint for retrieving repairs
        string url = $"{sourcePrefix}repairs/get_all_by_vat/{VATNum}";
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

    public async Task<IActionResult> GetUserRepairsByUID(int id)
    {
        if (id <= 0)
        {
            return View("Error", "Negative user Id Detected");
        }
        // Define the API endpoint for retrieving repairs
        string url = $"{sourcePrefix}repairs/get_all_by_id/{id}";
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
                return View("GetUserRepairs", apiResponse.Value);
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
            return RedirectToAction("GetUserRepairs");
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

        return RedirectToAction("GetUserRepairs", new { VATNum = deletedRepair?.Value?.OwnerVAT });
    }











    // CREATE

    [HttpPost]
    public async Task<ResponseApi<RepairDTO>?> CreateRepairToRedirectController(RepairDTO repair)
    {
        string url = $"{sourcePrefix}user/create_repair";
        var response = await client.PostAsJsonAsync(url, repair);
        var responseBody = await response.Content.ReadAsStringAsync();
        ResponseApi<RepairDTO>? targetRepair = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<RepairDTO>>(responseBody);
        return targetRepair;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRepairCallback(RepairDTO pendingCreationRepair)
    {
        //// Front end validation of the model
        //if (!ModelState.IsValid)
        //{
        //    // Return to the form view with data annotation error messages
        //    return View("UserCreateRepair", pendingCreationRepair);
        //}

        ResponseApi<RepairDTO>? createdRepair = await CreateRepairToRedirectController(pendingCreationRepair);

        if (createdRepair?.Value != null)
        {
            if (createdRepair?.Status == 0)
            {
                pendingCreationRepair.ErrorCode = 0;
                pendingCreationRepair.ErrorDescription = createdRepair.Description;
            }
            return View("UserCreateRepair", pendingCreationRepair);
        }
        else
        {
            if (createdRepair?.Status == 1)
            {
                pendingCreationRepair.ErrorCode = 1;
                pendingCreationRepair.ErrorDescription = createdRepair.Description;
            }
            return View("UserCreateRepair", pendingCreationRepair);
        }
    }







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

        return View("UserUpdateRepair", repairDetails?.Value);
    }














    [HttpPut]
    public async Task<ResponseApi<RepairDTO>?> UpdateUserToRedirectController(RepairDTO repair)
    {
        string url = $"{sourcePrefix}user/update_repair";
        var response = await client.PutAsJsonAsync(url, repair);
        var responseBody = await response.Content.ReadAsStringAsync();
        ResponseApi<RepairDTO>? targetRepair = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<RepairDTO>>(responseBody);
        return targetRepair;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRepairCallback(RepairDTO pendingUpdateRepair)
    {
        //// Front end validation of the model
        //if (!ModelState.IsValid)
        //{
        //    // Return to the form view with data annotation error messages
        //    return View("UserUpdateRepair", pendingUpdateRepair);
        //}

        ResponseApi<RepairDTO>? createdRepair = await UpdateUserToRedirectController(pendingUpdateRepair);

        if (createdRepair?.Value != null)
        {
            if (createdRepair?.Status == 0)
            {
                pendingUpdateRepair.ErrorCode = 0;
                pendingUpdateRepair.ErrorDescription = createdRepair.Description;
            }
            return View("UserUpdateRepair", pendingUpdateRepair);
        }
        else
        {
            if (createdRepair?.Status == 1)
            {
                pendingUpdateRepair.ErrorCode = 1;
                pendingUpdateRepair.ErrorDescription = createdRepair.Description;
            }
            return View("UserUpdateRepair", pendingUpdateRepair);
        }
    }






















    // View loading

    public IActionResult UserCreateRepair()
    {
        return View(new RepairDTO { ScheduledDate = DateTime.Now, PropertyIdNum = "", Cost = 0, Description = "" });
    }

    public IActionResult UserUpdateRepair()
    {
        return View();
    }

    public IActionResult ViewRepairDetails()
    {
        return View();
    }

}
