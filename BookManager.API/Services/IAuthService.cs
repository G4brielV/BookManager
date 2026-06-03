using BookManager.API.DTOs;

namespace BookManager.API.Services.Impl
{
    public interface IAuthService
    {
        Task<Result<RegisterResponse>> Register(RegisterRequest request);
        Task<Result<LoginResponse>> Login(LoginRequest request);
    }
}