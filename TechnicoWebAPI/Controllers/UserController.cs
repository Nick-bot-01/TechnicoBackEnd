using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechnicoBackEnd.DTOs;
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

        [HttpGet("users")]
        public async Task<ResponseApi<List<UserDTO>>> GetUsers() => await _userService.GetAllUsers();


        [HttpGet("users/{id}")]
        public async Task<ResponseApi<UserDTO>> GetCustomer([FromRoute] int id) => await _userService.GetUserDetailsById(id);

        [HttpPost("register_user")]
        public async Task<ResponseApi<UserDTO>> RegisterUser([FromBody] UserWithRequiredFieldsDTO user) => await _userService.Register(user);

        [HttpPut("update_user")] //todo change to async
        public ResponseApi<UserDTO> UpdateUser([FromBody] UserDTO user){
            return new ResponseApi<UserDTO> { Status = 1, Description = "UpdateUser Not Yet Implemented" };
        }

        [HttpPut("login_user")] //maybe this flag needs to change ??!?!
        public ResponseApi<UserDTO> LoginUser([FromBody] UserDTO user)//todo change to async
        {
            return new ResponseApi<UserDTO> { Status = 1, Description = "LoginUser Not Yet Implemented" };
        }

        [HttpDelete("delete_user_soft/{vat}")]
        public async Task<ResponseApi<UserDTO>> DeleteUserSoft([FromRoute] string? vat) => await _userService.DeleteOwnerSoft(vat);

        [HttpDelete("delete_user_hard/{vat}")]
        public async Task<ResponseApi<UserDTO>> DeleteUserHard([FromRoute] string? vat) => await _userService.DeleteOwnerHard(vat);

        [HttpGet("search_user")]
        public async Task<ResponseApi<UserDTO>> SearchUser([FromBody] string? vat, string? email){
            return await _userService.SearchUser(vat, email);
        }
    }
}
