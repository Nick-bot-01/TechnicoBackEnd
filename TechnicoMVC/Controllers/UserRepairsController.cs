using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Models;
using TechnicoBackEnd.Repositories;
using TechnicoBackEnd.Responses;

namespace TechnicoMVC.Controllers
{
    public class UserRepairsController : Controller
    {
        private readonly ILogger<RepairController> _logger;
        private readonly string sourcePrefix = "https://localhost:7017/api/Repair/"; //for other controller change to Repair / Property etc.
        private HttpClient client = new HttpClient();

        public UserRepairsController(ILogger<RepairController> logger) => _logger = logger;


        public async Task<IActionResult> GetUserRepairs(string VATNum) //https://localhost:7130/UserRepairs/GetUserRepairs
        {
            if (string.IsNullOrWhiteSpace(VATNum))
            {
                _logger.LogWarning("VATNum is null or empty.");
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
            _logger.LogError("API request failed with status code: {StatusCode} and message: {Message}", response.StatusCode, response.ReasonPhrase);
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
    }
}
