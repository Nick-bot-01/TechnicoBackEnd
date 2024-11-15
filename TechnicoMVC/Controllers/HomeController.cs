using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TechnicoMVC.Models;
using TechnicoWebAPI;
using TechnicoBackEnd.Models;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Responses;
using Microsoft.AspNetCore.Identity.Data;
using TechnicoBackEnd.Auth;

namespace TechnicoMVC.Controllers;

public class HomeController : Controller{
    private static UserDTO? _activeUser = null; 

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

    [HttpDelete]
    public async Task<ResponseApi<UserDTO>?> RemoveUserToRedirectController(string? vat)
    {
        string url = $"{sourcePrefix}delete_user_soft/{vat}";
        var response = await client.DeleteAsync(url);
        var responseBody = await response.Content.ReadAsStringAsync();
        ResponseApi<UserDTO>? removedUser = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<UserDTO>>(responseBody);
        return removedUser;
    }

    public async Task<IActionResult> Index() {
        int rnd = Random.Shared.Next(-10, 10);
        var result = await ReadUserToRedirectController(rnd);
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

    public async Task<IActionResult> RemoveUserCallback(string? vat)
    {
        if (string.IsNullOrEmpty(vat)) return RedirectToAction("Index"); //failsafe temp
        ResponseApi<UserDTO>? deletedUser = await RemoveUserToRedirectController(vat);
        return RedirectToAction("Index");
    }


    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });


    //Login Based Section
    [HttpPost]
    public async Task<IActionResult> LoginRequest(LoginRequest loginRequest){
        string url = $"{sourcePrefix}login";
        var response = await client.PostAsJsonAsync(url, loginRequest);
        if (response.IsSuccessStatusCode){
            var responseBody = await response.Content.ReadAsStringAsync();
            ResponseApi<UserDTO>? responseUserDTO = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<UserDTO>>(responseBody);
            if (responseUserDTO != null && responseUserDTO.Value!= null){
                LoginState.UserId = responseUserDTO.Value.Id;
                _activeUser = responseUserDTO.Value;
                return RedirectToAction("Index2");
            }
        }
        ModelState.AddModelError(string.Empty, "Invalid login credentials");
        return View();
    }

    public IActionResult Login() => View(_activeUser);

    public IActionResult Index2()
    {
        if (LoginState.UserId == -1) return RedirectToAction("Login"); // Redirect to Login if not logged in

        if (LoginState.IsAdmin) ViewBag.Message = "Welcome, Admin! You have access to admin-specific content.";
        else ViewBag.Message = $"Welcome, {_activeUser?.Name}! You have access to regular user content.";

        return View();
    }
}
