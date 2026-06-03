
using BookManager.API.DTOs;

namespace BookManager.API.Services
{
    public interface IBookService
    {
        Task<Result<IEnumerable<BookResponse>>> GetTodoListsAsync(int userId);
        Task<Result<BookResponse>> FindByIdAsync(int id, int userId);
        Task<Result<BookResponse>> CriarTodoListAsync(int id, BookRequest request);
        Task<Result<BookResponse>> UpdateTodoListAsync(int id, BookRequest request, int userId);
        Task<Result<bool>> DeleteTodoListAsync(int id, int userId);

    }
}