using BookManager.API.Models;
namespace BookManager.API.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}