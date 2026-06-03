using BookManager.API.DTOs;
using BookManager.API.Mappings;
using BookManager.API.Models;
using BookManager.API.Repositories;

namespace BookManager.API.Services.Impl

{
    public class AuthService(IUserRepository repository, ITokenService tokenService) : IAuthService
    {
        public async Task<Result<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
            // business rule: email must be unique
            if (await repository.EmailExistsAsync(request.Email))
            {
                return Result<RegisterResponse>.Failure("Este e-mail já está em uso."); 
            }
            string encryptedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            User user = request.ToEntity(encryptedPassword);
            await repository.AddUserAsync(user);

            return Result<RegisterResponse>.Success(user.ToResponse());
        }

        public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
        {   
            var user = await repository.FindByEmailAsync(request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return Result<LoginResponse>.Failure("E-mail ou senha incorretos.");
            }

            string token = tokenService.GenerateToken(user);

            var response = new LoginResponse(token);

            return Result<LoginResponse>.Success(response);
        }

    }
}