using BookManager.API.Data;
using BookManager.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BookManager.API.Repositories;

public class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;

    public BookRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Book> Books, int TotalCount)> GetBooksFromUserAsync(int userId, int pageNumber, int pageSize)
    {
        // Todos os livros do usuario
        var query = _context.Books.Where(b => b.UserId == userId);
        // Total de livros
        int totalCount = await query.CountAsync();
        // Paginação
        var books = await query
            .OrderBy(b => b.Title)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (books, totalCount);
    }

    public async Task<Book?> FindByIdAsync(int id, int userId)
    {
        return await _context.Books
            .SingleOrDefaultAsync(b => b.Id == id && b.UserId == userId);
    }

    public async Task<Book?> FindByTitleAsync(string title, int userId)
    {
        return await _context.Books
            .FirstOrDefaultAsync(b => b.Title.Contains(title) && b.UserId == userId);
    }

    public async Task AddBookAsync(Book book)
    {
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync(); 
    }

    public async Task UpdateBookAsync(Book book)
    {
        _context.Books.Update(book);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteBookAsync(Book book)
    {
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
    }
}