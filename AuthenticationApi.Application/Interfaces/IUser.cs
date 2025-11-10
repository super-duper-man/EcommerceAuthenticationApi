using AuthenticationApi.Application.Dtos;
using Resource.Share.Lib.Responses;

namespace AuthenticationApi.Application.Interfaces
{
    public interface IUser
    {
        Task<Response> Register(AppUserDto appUserDto);
        Task<Response> Login(LoginDto loginDto);
        Task<AppUserDto> GetUser(int userId);
    }
}
