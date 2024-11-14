using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TechnicoMVC.Models;
using TechnicoWebAPI;
using TechnicoBackEnd.Models;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Responses;

namespace TechnicoMVC.Controllers;

public class HomeController : Controller{
    private readonly ILogger<HomeController> _logger;
    private readonly string sourcePrefix = "https://localhost:7017/api/User/";
    private HttpClient client = new HttpClient();

    public HomeController(ILogger<HomeController> logger)=> _logger = logger;

    //Test only
    [HttpGet]
    public async Task<ResponseApi<UserDTO>?> ReadUserToRedirectController(int id)
    {
        HttpClient client = new HttpClient();
        string url = $"{sourcePrefix}users/{id}";
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseBody.ToString());
        ResponseApi<UserDTO>? targetUser = System.Text.Json.JsonSerializer.Deserialize<ResponseApi<UserDTO>>(responseBody);
        Console.WriteLine($"Status: {targetUser?.Status} Description: {targetUser?.Description}");

        return targetUser;
    }


    public async Task<IActionResult> Index() {
        int rnd = Random.Shared.Next(-10, 10);
        var result = await ReadUserToRedirectController(rnd);
        return View(result);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
