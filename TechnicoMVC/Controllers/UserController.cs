using Microsoft.AspNetCore.Mvc;
using TechnicoBackEnd.Auth;

namespace TechnicoMVC.Controllers;

public class UserController : Controller{
    private readonly ILogger<UserController> _logger;
    private readonly string sourcePrefix = "https://localhost:7017/api/"; //for other controller change to Repair / Property etc.
    private HttpClient client = new HttpClient();
    public UserController(ILogger<UserController> logger) => _logger = logger;

    //Web Api Callbacks




    //Views
    public ActionResult UserHome(){
        if (!LoginState.IsLoggedIn) return RedirectToAction("LandingPage");

        if (!LoginState.IsAdmin) return View();
        else return RedirectToAction("LandingPage");
    }



    //View Callbacks
}
