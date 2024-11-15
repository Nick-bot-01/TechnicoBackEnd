using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using TechnicoBackEnd.Auth;
using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Responses;
using TechnicoBackEnd.Services;

namespace TechnicoWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase{
    private readonly IUserService _userService;

    public UserController(IUserService userService) => _userService = userService;

    [HttpPost("login")]
    public async Task<ResponseApi<UserDTO>> Login([FromBody] LoginRequest loginRequest){
        var user = await _userService.Authenticate(loginRequest.Email, loginRequest.Password);
        return user;
    }

    [HttpPost("checkAdmin")]
    public async Task<ResponseApi<bool>> IsAdmin([FromBody] string email)
    {
        var result = await _userService.IsAdmin(email);
        return result;
    }

    [HttpPost("logout")]
    public ResponseApi<UserDTO> Logout(){
        return new ResponseApi<UserDTO>() { Status = 1 , Description = "Log out!"};
    }

    [HttpGet("users")]
    public async Task<ResponseApi<List<UserDTO>>> GetUsers() => await _userService.GetAllUsers();

    [HttpGet("users/{id}")]
    public async Task<ResponseApi<UserDTO>> GetCustomer([FromRoute] int id) => await _userService.GetUserDetailsById(id);

    [HttpPost("register_user")]
    public async Task<ResponseApi<UserDTO>> RegisterUser([FromBody] UserWithRequiredFieldsDTO user) => await _userService.Register(user);

    [HttpPut("update_user")]
    public async Task<ResponseApi<UserDTO>> UpdateUser([FromBody] UserWithRequiredFieldsDTO user) => await _userService.Update(user);

    [HttpDelete("delete_user_soft/{vat}")]
    public async Task<ResponseApi<UserDTO>> DeleteUserSoft([FromRoute] string? vat) => await _userService.DeleteSoft(vat);

    [HttpDelete("delete_user_hard/{vat}")]
    public async Task<ResponseApi<UserDTO>> DeleteUserHard([FromRoute] string? vat) => await _userService.DeleteHard(vat);

    [HttpGet("search_user")]
    public async Task<ResponseApi<UserDTO>> SearchUser(string? vat, string? email){
        return await _userService.Search(vat, email);
    }
}
