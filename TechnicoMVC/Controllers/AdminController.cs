using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using TechnicoBackEnd.Auth;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Responses;
using TechnicoMVC.ViewModels;

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
    public async Task<UserDTO> GetOwnerDetails(int? id)
    {
        if (id.HasValue){
            string url = $"{sourcePrefix}User/users/{id}";
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            ResponseApi<UserDTO>? userResponse = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<UserDTO>>(responseBody);
            if (userResponse == null || userResponse.Value == null) return new UserDTO();
            return userResponse.Value;
        }
        else return new UserDTO();
    }

    [HttpPut]
    public async Task<UserDTO?> UpdateOwner(UserDTO userDTO)
    {
        string url = $"{sourcePrefix}User/update_user";
        var response = await client.PutAsJsonAsync(url, userDTO);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        ResponseApi<UserDTO>? createdResponseUserDTO = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<UserDTO>>(responseBody);
        return createdResponseUserDTO?.Value;
    }

    [HttpPost]
    public async Task<UserDTO?> SearchOwnerByVatOrEmail(string? searchInput)
    {
        QueryUserDTO queryUserDTO = new QueryUserDTO() { VAT = searchInput, Email = searchInput };
        string url = $"{sourcePrefix}User/search_user";
        var response = await client.PostAsJsonAsync(url, queryUserDTO);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        ResponseApi<UserDTO>? userResponse = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<UserDTO>>(responseBody);
        return userResponse?.Value;
    }

    
    [HttpDelete]
    public async Task<ResponseApi<UserDTO>?> DeleteOwner(string? ownerVat)
    {
        string url = $"{sourcePrefix}User/delete_user_soft/{ownerVat}";
        var response = await client.DeleteAsync(url);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        ResponseApi<UserDTO>? createdResponseUserDTO = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<UserDTO>>(responseBody);
        return createdResponseUserDTO;
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

    public IActionResult EditOwner()
    {
        if (!LoginState.IsLoggedIn) return RedirectToAction("LandingPage");

        if (LoginState.IsAdmin)
        {
                return View();
        }
        else return RedirectToAction("LandingPage");
    }
    //probably not needed, cant find a corresponding view
    public IActionResult AdminUsersAndProperties()
    {
        if (!LoginState.IsLoggedIn) return RedirectToAction("LandingPage");

        if (LoginState.IsAdmin) return View(new ActiveUserViewModel() { Name = LoginState.activeUser?.Name });
        else return RedirectToAction("LandingPage");
    }

    public IActionResult SearchOwner()
    {
        if (!LoginState.IsLoggedIn) return RedirectToAction("LandingPage");

        if (LoginState.IsAdmin)
        {
            return View();
        }
        else return RedirectToAction("LandingPage");
    }

    public IActionResult SearchOwnerResult(UserDTO? user)
    {
        if (!LoginState.IsLoggedIn) return RedirectToAction("LandingPage");

        if (LoginState.IsAdmin)
        {
            return View(user);
        }
        else return RedirectToAction("LandingPage");
    }

    //Views Callbacks

    public async Task<IActionResult> PresentTargetOwnerDetailsCallback(int ownerId)
    {
        var result = await GetOwnerDetails(ownerId);
        return View("EditOwner", result);
    }

    public async Task<IActionResult> UpdateOwnerCallback(UserDTO userDTO)
    {
        var result = await UpdateOwner(userDTO);
        if (result != null && result.Email != null) return RedirectToAction("OwnersAndProperties");
        return RedirectToAction("EditOwner", userDTO.Id);
    }

    public async Task<IActionResult> SearchOwnerResultsCallback(string? searchInput)
    {
        var result = await SearchOwnerByVatOrEmail(searchInput);
        return View("SearchOwnerResult", result);
    }

    public async Task<IActionResult> DeleteOwnerCallback(string? ownerVat)
    {
        var result = await DeleteOwner(ownerVat);
        return RedirectToAction("OwnersAndProperties");
    }

    public async Task<IActionResult> SearchProperties(string? PIN, string? VAT)
    {
        string url = $"{sourcePrefix}Property/search_properties";

        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(PIN)) { queryParams.Add($"pin={PIN}"); }
        if (!string.IsNullOrEmpty(VAT)) { queryParams.Add($"vat={VAT}"); }
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
            var apiResponse = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<List<PropertyDTO>>>(responseBody, options);

            if (apiResponse?.Value != null)
            {
                return View(apiResponse.Value);
            }
        }

        return View("Error");
    }
}
