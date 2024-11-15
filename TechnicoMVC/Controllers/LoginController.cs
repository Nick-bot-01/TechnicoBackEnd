﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using TechnicoBackEnd.Auth;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Responses;

namespace TechnicoMVC.Controllers;

public class LoginController : Controller{
    private readonly ILogger<LoginController> _logger;
    private readonly string sourcePrefix = "https://localhost:7017/api/"; //for other controller change to Repair / Property etc.
    private HttpClient client = new HttpClient();

    public LoginController(ILogger<LoginController> logger) => _logger = logger;

    //Web API Wrapper callbacks
    [HttpPost]
    public async Task<ResponseApi<bool>?> IsAdmin(string? email){
        string url = $"{sourcePrefix}User/checkAdmin";
        var response = await client.PostAsJsonAsync(url, email);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        ResponseApi<bool>? isAdminResponse = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<bool>>(responseBody);
        return isAdminResponse;
    }


    [HttpPost]
    public async Task<IActionResult> LoginRequest(LoginRequest loginRequest){
        string url = $"{sourcePrefix}User/login";
        var response = await client.PostAsJsonAsync(url, loginRequest);
        if (response.IsSuccessStatusCode){
            var responseBody = await response.Content.ReadAsStringAsync();
            ResponseApi<UserDTO>? responseUserDTO = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<UserDTO>>(responseBody);
            if (responseUserDTO != null && responseUserDTO.Value != null){
                LoginState.UserId = responseUserDTO.Value.Id;
                LoginState.activeUser = responseUserDTO.Value;
                LoginState.IsLoggedIn = true;
                var result = await IsAdmin(responseUserDTO.Value.Email);
                LoginState.IsAdmin = (result != null) ? result.Value : false;
                if (LoginState.IsAdmin){
                    return RedirectToAction("AdminHome");
                }
                else
                {
                    return RedirectToAction("UserHome");
                }
                
            }
        }
        ModelState.AddModelError(string.Empty, "Invalid login credentials");
        return RedirectToAction("Login");
    }


    //Views Loading
    public ActionResult LandingPage(){
        if (LoginState.IsLoggedIn){
            if(LoginState.IsAdmin) return RedirectToAction("AdminHome");
            else return RedirectToAction("UserHome");
        }
        return View();
    }

    //This should be in the respective controler for AdminController
    public ActionResult AdminHome()
    {
        return View();
    }

    //This should be in the respective controler for UserController
    public ActionResult UserHome()
    {
        return View();
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