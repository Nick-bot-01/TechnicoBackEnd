using Microsoft.AspNetCore.Mvc;
using TechnicoBackEnd.Auth;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Models;
using TechnicoBackEnd.Responses;

namespace TechnicoMVC.Controllers;

public class UserController : Controller{
    private readonly ILogger<UserController> _logger;
    private readonly string sourcePrefix = "https://localhost:7017/api/"; //for other controller change to Repair / Property etc.
    private HttpClient client = new HttpClient();
    public UserController(ILogger<UserController> logger) => _logger = logger;

    //Web Api Callbacks
    [HttpGet]
    public async Task<ResponseApi<List<RepairDTO>>?> GetUserRepairsToRedirectController(){
        string url = $"{sourcePrefix}Repair/repairs/get_all_by_vat/{LoginState.activeUser?.VAT}";
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        ResponseApi<List<RepairDTO>>? userRepairResponse = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<List<RepairDTO>>>(responseBody);
        return userRepairResponse;
    }

    [HttpPut]
    public async Task<ResponseApi<UserDTO>?> UpdateUserToRedirectController(UserDTO user){
        string url = $"{sourcePrefix}User/update_user";
        var response = await client.PutAsJsonAsync(url, user);
        var responseBody = await response.Content.ReadAsStringAsync();
        ResponseApi<UserDTO>? targetUser = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<UserDTO>>(responseBody);
        return targetUser;
    }

    [HttpDelete]
    public async Task<ResponseApi<UserDTO>?> RemoveUserToRedirectController(string? vat){
        string url = $"{sourcePrefix}User/delete_user_soft/{vat}";
        var response = await client.DeleteAsync(url);
        var responseBody = await response.Content.ReadAsStringAsync();
        ResponseApi<UserDTO>? removedUser = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<UserDTO>>(responseBody);
        return removedUser;
    }

    [HttpGet]
    public async Task RefreshActiveUserData(){
        string url = $"{sourcePrefix}User/users/{LoginState.UserId}";
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        ResponseApi<UserDTO>? targetUser = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<UserDTO>>(responseBody);
        LoginState.activeUser = targetUser?.Value;
        if(LoginState.activeUser != null) LoginState.UserId = LoginState.activeUser.Id;
    } 


    //Views
    public async Task<IActionResult> UserHome(){
        if (!LoginState.IsLoggedIn) return RedirectToAction("LandingPage");

        if (!LoginState.IsAdmin){
            var userRepairsResponse = await GetUserRepairsToRedirectController();
            return View(userRepairsResponse);
        }
        else return RedirectToAction("LandingPage");
    }

    public IActionResult UserDetails(){
        if (!LoginState.IsLoggedIn) return RedirectToAction("LandingPage");

        if (!LoginState.IsAdmin){
            return View();
        }
        else return RedirectToAction("LandingPage");
    }

    public IActionResult UserUpdate(){
        if (!LoginState.IsLoggedIn) return RedirectToAction("LandingPage");

        if (!LoginState.IsAdmin)
        {
            return View(LoginState.activeUser);
        }
        else return RedirectToAction("LandingPage");
    }


    //View Callbacks
    [HttpPost]
    public async Task<IActionResult> UserUpdateButtonCallback(UserDTO userDTO){
        var response = await UpdateUserToRedirectController(userDTO);
        await RefreshActiveUserData();
        return RedirectToAction("UserHome");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUserButtonCallback(UserDTO userDTO){      
        var result = await RemoveUserToRedirectController(userDTO?.VAT);
        return RedirectToAction("Logout", "Login");
    }
}
