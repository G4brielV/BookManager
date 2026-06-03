using BookManager.API.DTOs;
using BookManager.API.Models;

namespace BookManager.API.Mappings;

public static class BookMapper
{
    public static BookResponse ToResponse(this Book book)
    {
        return new BookResponse
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            PublishDate = book.PublishDate
        };
    }
    public static Book ToEntity(this BookRequest request, int userId)
    {
        return new Book
        {
            Title = request.Title,
            Author = request.Author,
            PublishDate = request.PublishDate,
            UserId = userId 
        };
    }
}