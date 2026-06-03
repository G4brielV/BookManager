using BookManager.API.DTOs;

namespace BookManager.API.Services
{
    public interface IUsuarioService
    {
        Task<Result<UserResponse>> GetByIdAsync(int id);

    }
}