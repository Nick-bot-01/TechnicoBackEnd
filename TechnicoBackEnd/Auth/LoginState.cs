using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnicoBackEnd.DTOs;
namespace TechnicoBackEnd.Auth;
public static class LoginState{
    public static int UserId { get; set; } = -1; // -1 indicates no user logged in
    public static bool IsLoggedIn { get; set; } = false;
    public static bool IsAdmin { get; set; } = false;
    public static UserDTO? activeUser { get; set; }
}
