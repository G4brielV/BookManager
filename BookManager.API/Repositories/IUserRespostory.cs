using BookManager.API.Models;

namespace BookManager.API.Repositories
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task<bool> EmailExistsAsync(string email);
        Task<User?> FindByIdAsync(int id);
        Task<User?> FindByEmailAsync(string email);

    }
}