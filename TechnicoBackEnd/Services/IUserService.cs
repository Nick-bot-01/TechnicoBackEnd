using TechnicoBackEnd.DTOs;
using TechnicoBackEnd.Responses;

namespace TechnicoBackEnd.Services
{
    public interface IUserService
    {
        Task<ResponseApi<UserDTO>> DeleteHard(string? vat);
        Task<ResponseApi<UserDTO>> DeleteSoft(string? vat);
        Task<ResponseApi<List<UserDTO>>> GetAllUsers();
        Task<ResponseApi<UserDTO>> GetUserDetailsById(int id);
        Task<ResponseApi<UserDTO>> Register(UserWithRequiredFieldsDTO userDto);
        Task<ResponseApi<UserDTO>> Search(string? vat, string? email);
        Task<ResponseApi<UserDTO>> Update(UserWithRequiredFieldsDTO userDto);
        Task<ResponseApi<UserDTO>> Authenticate(string email, string password);
        Task<ResponseApi<bool>> IsAdmin(string email);
    }
}