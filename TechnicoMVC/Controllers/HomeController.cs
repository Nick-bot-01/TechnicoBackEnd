using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TechnicoMVC.Models;
using TechnicoWebAPI;
using TechnicoBackEnd.Models;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Responses;
using TechnicoBackEnd.Auth;
using Microsoft.AspNetCore.Identity.Data;
using TechnicoBackEnd.Services;

namespace TechnicoMVC.Controllers;

public class HomeController : Controller{
    private readonly ILogger<HomeController> _logger;
    private readonly string sourcePrefix = "https://localhost:7017/api/User/"; //for other controller change to Repair / Property etc.
    private HttpClient client = new HttpClient();

    public HomeController(ILogger<HomeController> logger)=> _logger = logger;


    //Test only
    [HttpGet]
    public async Task<ResponseApi<UserDTO>?> ReadUserToRedirectController(int id)
    {
        string url = $"{sourcePrefix}users/{id}";
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        ResponseApi<UserDTO>? targetUser = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<UserDTO>>(responseBody);
        return targetUser;
    }

    [HttpPost]
    public async Task<ResponseApi<UserDTO>?> CreateUserToRedirectController(UserWithRequiredFieldsDTO user){
        string url = $"{sourcePrefix}register_user";
        var response = await client.PostAsJsonAsync(url, user);
        var responseBody = await response.Content.ReadAsStringAsync();
        ResponseApi<UserDTO>? targetUser = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<UserDTO>>(responseBody);
        return targetUser;
    }
    [HttpPut]
    public async Task<ResponseApi<UserDTO>?> UpdateUserToRedirectController(UserWithRequiredFieldsDTO user)
    {
        string url = $"{sourcePrefix}update_user";
        var response = await client.PutAsJsonAsync(url, user);
        var responseBody = await response.Content.ReadAsStringAsync();
        ResponseApi<UserDTO>? targetUser = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<UserDTO>>(responseBody);
        Console.WriteLine($"Status: {targetUser?.Status} Description: {targetUser?.Description}");
        return targetUser;
    }


    public IActionResult Index() {
        //int rnd = Random.Shared.Next(-10, 10);
        //var result = await ReadUserToRedirectController(rnd);
        return View();
    }

    //Callback from Add User
    public async Task<IActionResult> CreateUserCallback(UserWithRequiredFieldsDTO pendingCreationUser){
        ResponseApi<UserDTO>? createdUSer = await CreateUserToRedirectController(pendingCreationUser);
        return RedirectToAction("Index");
    }

    //Callback from Update User
    public async Task<IActionResult> UpdateUserCallback(UserWithRequiredFieldsDTO pendingCreationUser)
    {
        ResponseApi<UserDTO>? createdUSer = await UpdateUserToRedirectController(pendingCreationUser);
        return RedirectToAction("Index");
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });




    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        string url = $"{sourcePrefix}login";
        var response = await client.PostAsJsonAsync(url, loginRequest);

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();

            using var document = System.Text.Json.JsonDocument.Parse(responseBody);
            if (document.RootElement.TryGetProperty("Value", out var valueElement))
            {
                // Deserialize only the "Value" property to User
                var user = System.Text.Json.JsonSerializer.Deserialize<User>(valueElement.GetRawText());
                if (user != null)
                {
                    LoginState.UserId = user.Id;
                    return RedirectToAction("Index2");
                }
            }
        }

        ModelState.AddModelError(string.Empty, "Invalid login credentials");
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Index2()
    {
        if (LoginState.UserId == -1)
        {
            return RedirectToAction("Login"); // Redirect to Login if not logged in
        }

        if (LoginState.IsAdmin)
        {
            ViewBag.Message = "Welcome, Admin! You have access to admin-specific content.";
            //return View("");
        }
        else
        {
            ViewBag.Message = "Welcome, User! You have access to regular user content.";
            //return View();
        }

        return View();
    }
}
