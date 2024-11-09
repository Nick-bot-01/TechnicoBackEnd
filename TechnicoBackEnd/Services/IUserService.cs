using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Responses;

namespace TechnicoBackEnd.Services
{
    public interface IUserService
    {
        Task<ResponseApi<UserDTO>> DeleteOwnerHard(string? vat);
        Task<ResponseApi<UserDTO>> DeleteOwnerSoft(string? vat);
        Task<ResponseApi<List<UserDTO>>> GetAllUsers();
        Task<ResponseApi<UserDTO>> GetUserDetailsById(int id);
        Task<ResponseApi<UserDTO>> Register(UserWithRequiredFieldsDTO userDto);
        Task<ResponseApi<UserDTO>> SearchUser(string? vat, string? email);
    }
}