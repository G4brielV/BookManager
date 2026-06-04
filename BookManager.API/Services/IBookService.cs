using BookManager.API.DTOs;

namespace BookManager.API.Services
{
    public interface IBookService
    {
        Task<Result<PaginatedResponse<BookResponse>>> GetBooksAsync(int userId, int pageNumber, int pageSize);
        Task<Result<BookResponse>> FindByIdAsync(int id, int userId);
        Task<Result<BookResponse>> CreateBookAsync(int userId, BookRequest request);
        Task<Result<BookResponse>> UpdateBookAsync(int id, BookRequest request, int userId);
        Task<Result<bool>> DeleteBookAsync(int id, int userId);

    }
}