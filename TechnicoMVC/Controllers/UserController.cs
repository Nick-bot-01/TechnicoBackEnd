using Microsoft.AspNetCore.Mvc;
using TechnicoBackEnd.Auth;

namespace TechnicoMVC.Controllers;

public class UserController : Controller{

    public ActionResult UserHome(){
        if (!LoginState.IsLoggedIn) return RedirectToAction("LandingPage");

        if (!LoginState.IsAdmin) return View();
        else return RedirectToAction("LandingPage");
    }
}
