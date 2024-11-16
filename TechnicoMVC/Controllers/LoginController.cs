﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using TechnicoBackEnd.Auth;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Responses;
using TechnicoMVC.ViewModels;

namespace TechnicoMVC.Controllers;

public class LoginController : Controller {
    private readonly ILogger<LoginController> _logger;
    private readonly string sourcePrefix = "https://localhost:7017/api/"; //for other controller change to Repair / Property etc.
    private HttpClient client = new HttpClient();

    public LoginController(ILogger<LoginController> logger) => _logger = logger;

    //Web API Wrapper callbacks
    [HttpPost]
    public async Task<ResponseApi<bool>?> IsAdmin(string? email) {
        string url = $"{sourcePrefix}User/checkAdmin";
        var response = await client.PostAsJsonAsync(url, email);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        ResponseApi<bool>? isAdminResponse = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<bool>>(responseBody);
        return isAdminResponse;
    }

    [HttpPost]
    public async Task<IActionResult> RegisterRequest(UserWithRequiredFieldsDTO userWithRequiredFieldsDTO) {
        string url = $"{sourcePrefix}User/register_user";
        var response = await client.PostAsJsonAsync(url, userWithRequiredFieldsDTO);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        ResponseApi<UserDTO>? createdResponseUserDTO = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<UserDTO>>(responseBody);
        if (userWithRequiredFieldsDTO != null && userWithRequiredFieldsDTO.Email != null && userWithRequiredFieldsDTO.Password != null) {
            LoginRequest postRegistrationLoginRequest = new LoginRequest() { Email = userWithRequiredFieldsDTO.Email, Password = userWithRequiredFieldsDTO.Password };
            return await LoginRequest(postRegistrationLoginRequest);
        }
        return RedirectToAction("RegisterPage");
    }

    [HttpPost]
    public async Task<IActionResult> LoginRequest(LoginRequest loginRequest) {
        string url = $"{sourcePrefix}User/login";
        var response = await client.PostAsJsonAsync(url, loginRequest);
        if (response.IsSuccessStatusCode) {
            var responseBody = await response.Content.ReadAsStringAsync();
            ResponseApi<UserDTO>? responseUserDTO = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<UserDTO>>(responseBody);
            if (responseUserDTO != null && responseUserDTO.Value != null) {
                LoginState.UserId = responseUserDTO.Value.Id;
                LoginState.activeUser = responseUserDTO.Value;
                LoginState.IsLoggedIn = true;
                var result = await IsAdmin(responseUserDTO.Value.Email);
                LoginState.IsAdmin = (result != null) ? result.Value : false;
                if (LoginState.IsAdmin) {
                    return RedirectToAction("AdminHome");
                }
                else
                {
                    return RedirectToAction("UserHome", "User");
                }
            }
        }
        //ModelState.AddModelError(string.Empty, "Invalid login credentials");
        return RedirectToAction("LandingPage");
    }

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


    //Views Loading
    public ActionResult LandingPage() {
        if (LoginState.IsLoggedIn) {
            if (LoginState.IsAdmin) return RedirectToAction("AdminHome");
            else return RedirectToAction("UserHome", "User");
        }
        return View(new ActiveUserViewModel() { Name = LoginState.activeUser?.Name });
    }

    public ActionResult RegisterPage() {
        if (!LoginState.IsLoggedIn) return View();
        else return RedirectToAction("LandingPage");
    }

    //This should be in the respective controler for AdminController
    public async Task<ActionResult> AdminHome() {
        if (!LoginState.IsLoggedIn) return RedirectToAction("LandingPage");

        if (LoginState.IsAdmin) {
            var dailyRepairs = await GetDailyRepairs();
            return View(dailyRepairs);
        }
        else return RedirectToAction("LandingPage");
    }

    public ActionResult AdminUsersAndProperties(){
        if (!LoginState.IsLoggedIn) return RedirectToAction("LandingPage");

        if (LoginState.IsAdmin) return View(new ActiveUserViewModel() { Name = LoginState.activeUser?.Name });
        else return RedirectToAction("LandingPage");
    }

    

    //Frontend Callbacks
    public IActionResult Logout(){
        LoginState.UserId = -1;
        LoginState.IsLoggedIn = false;
        LoginState.IsAdmin = false;
        LoginState.activeUser = null;
        return RedirectToAction("LandingPage");
    }
}
