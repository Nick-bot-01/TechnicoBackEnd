using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using TechnicoBackEnd.Auth;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Responses;

namespace TechnicoMVC.Controllers;

public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;
    private readonly string sourcePrefix = "https://localhost:7017/api/"; //for other controller change to Repair / Property etc.
    private HttpClient client = new HttpClient();

    public AdminController(ILogger<AdminController> logger) => _logger = logger;

    //Web API Wrapper callbacks
    [HttpGet]
    public async Task<List<RepairDTO>> GetDailyRepairs()
    {
        string url = $"{sourcePrefix}Repair/repairs/get_all_daily";
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        ResponseApi<List<RepairDTO>>? dailyRepairsResponse = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<List<RepairDTO>>>(responseBody);
        if (dailyRepairsResponse == null || dailyRepairsResponse.Value == null) return new List<RepairDTO>();
        return dailyRepairsResponse.Value;
    }


    [HttpGet]
    public async Task<List<PropertyDTO>> GetPropertiesList()
    {
        string url = $"{sourcePrefix}Property/properties";
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        ResponseApi<List<PropertyDTO>>? propertiesResponse = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<List<PropertyDTO>>>(responseBody);
        if (propertiesResponse == null || propertiesResponse.Value == null) return new List<PropertyDTO>();
        return propertiesResponse.Value;
    }

    
    [HttpPost]
    public async Task<IActionResult> SubmitNewOwner(UserWithRequiredFieldsDTO userWithRequiredFieldsDTO)
    {
        string url = $"{sourcePrefix}User/register_user";
        var response = await client.PostAsJsonAsync(url, userWithRequiredFieldsDTO);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        ResponseApi<UserDTO>? createdResponseUserDTO = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<UserDTO>>(responseBody);
        if (userWithRequiredFieldsDTO != null && userWithRequiredFieldsDTO.Email != null && userWithRequiredFieldsDTO.Password != null)
        {
            return RedirectToAction("OwnersAndProperties");
        }
        return RedirectToAction("CreateOwner");
    }

    [HttpGet]
    public async Task<UserWithRequiredFieldsDTO> GetOwnerDetails(int? id)
    {
        if (id.HasValue){
            string url = $"{sourcePrefix}User/users/{id}";
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            ResponseApi<UserWithRequiredFieldsDTO>? userResponse = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<UserWithRequiredFieldsDTO>>(responseBody);
            if (userResponse == null || userResponse.Value == null) return new UserWithRequiredFieldsDTO();
            return userResponse.Value;
        }
        else return new UserWithRequiredFieldsDTO();
    }

    [HttpPost]
    public async Task<IActionResult> UpdateOwner(UserWithRequiredFieldsDTO userWithRequiredFieldsDTO)
    {
        string url = $"{sourcePrefix}User/update_user";
        var response = await client.PostAsJsonAsync(url, userWithRequiredFieldsDTO);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        ResponseApi<UserDTO>? createdResponseUserDTO = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<UserDTO>>(responseBody);
        if (userWithRequiredFieldsDTO != null && userWithRequiredFieldsDTO.Email != null && userWithRequiredFieldsDTO.Password != null)
        {
            return RedirectToAction("OwnersAndProperties");
        }
        return RedirectToAction("EditOwner", userWithRequiredFieldsDTO.Id);
    }

    //Views Loading
    public async Task<IActionResult> AdminHome()
    {
        if (!LoginState.IsLoggedIn) return RedirectToAction("LandingPage");

        if (LoginState.IsAdmin)
        {
            var dailyRepairs = await GetDailyRepairs();
            return View(dailyRepairs);
        }
        else return RedirectToAction("LandingPage");
    }

    public async Task<IActionResult> OwnersAndProperties()
    {
        if (!LoginState.IsLoggedIn) return RedirectToAction("LandingPage");

        if (LoginState.IsAdmin)
        {
            var propertiesList = await GetPropertiesList();
            return View(propertiesList);
        }
        else return RedirectToAction("LandingPage");
    }

    public IActionResult CreateOwner()
    {
        if (!LoginState.IsLoggedIn) return RedirectToAction("LandingPage");

        if (LoginState.IsAdmin)
        {
            return View();
        }
        else return RedirectToAction("LandingPage");
    }

    public async Task<IActionResult> EditOwner(int? id)
    {
        if (!LoginState.IsLoggedIn) return RedirectToAction("LandingPage");

        if (LoginState.IsAdmin)
        {
                var ownerDetails = await GetOwnerDetails(id);
                return View(ownerDetails);
        }
        else return RedirectToAction("LandingPage");
    }

}
