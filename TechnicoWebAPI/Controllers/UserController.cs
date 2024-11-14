using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TechnicoBackEnd.Auth;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Models;
using TechnicoBackEnd.Responses;
using TechnicoBackEnd.Services;

namespace TechnicoWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService) => _userService = userService;





        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            var user = _userService.Authenticate(loginRequest.Email, loginRequest.Password);
            if (user != null)
            {
                //LoginState.UserId = user.Id;
                //LoginState.IsLoggedIn = true;
                //LoginState.IsAdmin = user.Type == UserType.Admin; // Check if the user is an admin
                return Ok(new { Message = "Login successful", Value = user });
            }

            return Unauthorized(new { Message = "Invalid credentials" });
        }

        








        [HttpPost("logout")]
        public IActionResult Logout()
        {
            LoginState.UserId = -1;
            LoginState.IsLoggedIn = false;
            LoginState.IsAdmin = false;
            return Ok(new { Message = "Logout successful" });
        }






        [HttpGet("users")]
        public async Task<ResponseApi<List<UserDTO>>> GetUsers() => await _userService.GetAllUsers();


        [HttpGet("users/{id}")]
        public async Task<ResponseApi<UserDTO>> GetCustomer([FromRoute] int id) => await _userService.GetUserDetailsById(id);

        [HttpPost("register_user")]
        public async Task<ResponseApi<UserDTO>> RegisterUser([FromBody] UserWithRequiredFieldsDTO user) => await _userService.Register(user);

        [HttpPut("update_user")]
        public async Task<ResponseApi<UserDTO>> UpdateUser([FromBody] UserWithRequiredFieldsDTO user) => await _userService.Update(user);

        [HttpPut("login_user")] //maybe this flag needs to change ??!?!
        public ResponseApi<UserDTO> LoginUser([FromBody] UserDTO user)//todo change to async
        {
            return new ResponseApi<UserDTO> { Status = 1, Description = "LoginUser Not Yet Implemented" };
        }

        [HttpDelete("delete_user_soft/{vat}")]
        public async Task<ResponseApi<UserDTO>> DeleteUserSoft([FromRoute] string? vat) => await _userService.DeleteSoft(vat);

        [HttpDelete("delete_user_hard/{vat}")]
        public async Task<ResponseApi<UserDTO>> DeleteUserHard([FromRoute] string? vat) => await _userService.DeleteHard(vat);

        [HttpGet("search_user")]
        public async Task<ResponseApi<UserDTO>> SearchUser(string? vat, string? email){
            return await _userService.Search(vat, email);
        }
    }
}
