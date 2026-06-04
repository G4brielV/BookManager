using BookManager.API.DTOs;
using BookManager.API.Mappings;
using BookManager.API.Repositories;
using BookManager.API.Services;

namespace BookManager.API.Services.Impl;

public class BookService(IBookRepository repository) : IBookService
{
    public async Task<Result<PaginatedResponse<BookResponse>>> GetBooksAsync(int userId, int pageNumber, int pageSize)
    {
        var (books, totalCount) = await repository.GetBooksFromUserAsync(userId, pageNumber, pageSize);

        var response = new PaginatedResponse<BookResponse>
        {
            Items = books.Select(b => b.ToResponse()),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return Result<PaginatedResponse<BookResponse>>.Success(response);
    }

    public async Task<Result<BookResponse>> FindByIdAsync(int id, int userId)
    {
        var book = await repository.FindByIdAsync(id, userId);

        if (book == null)
        {
            return Result<BookResponse>.Failure("Livro não encontrado.");
        }

        return Result<BookResponse>.Success(book.ToResponse());
    }

    public async Task<Result<BookResponse>> CreateBookAsync(int userId, BookRequest request)
    {
        // Business rule: no duplicate title for the same user
        var existing = await repository.FindByTitleAsync(request.Title, userId);
        if (existing != null)
        {
            return Result<BookResponse>.Failure("Já existe um livro com este título cadastrado.");
        }

        var book = request.ToEntity(userId);

        await repository.AddBookAsync(book);

        return Result<BookResponse>.Success(book.ToResponse());
    }

    public async Task<Result<BookResponse>> UpdateBookAsync(int id, BookRequest request, int userId)
    {
        var book = await repository.FindByIdAsync(id, userId);

        if (book == null)
        {
            return Result<BookResponse>.Failure("Livro não encontrado.");
        }

        // Business rule: no duplicate title for the same user (excluding current book)
        var existing = await repository.FindByTitleAsync(request.Title, userId);
        if (existing != null && existing.Id != id)
        {
            return Result<BookResponse>.Failure("Já existe um livro com este título cadastrado.");
        }

        book.Title = request.Title;
        book.Author = request.Author;
        book.PublishDate = request.PublishDate;

        await repository.UpdateBookAsync(book);

        return Result<BookResponse>.Success(book.ToResponse());
    }

    public async Task<Result<bool>> DeleteBookAsync(int id, int userId)
    {
        var book = await repository.FindByIdAsync(id, userId);

        if (book == null)
        {
            return Result<bool>.Failure("Livro não encontrado.");
        }

        await repository.DeleteBookAsync(book);

        return Result<bool>.Success(true);
    }
}
