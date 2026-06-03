using BookManager.API.DTOs;

namespace BookManager.API.Services
{
    public interface IUsuarioService
    {
        Task<Result<UserResponse>> GetById(int id);

    }
}