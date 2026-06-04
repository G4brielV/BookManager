using BookManager.API.Models;

namespace BookManager.API.Repositories
{
    public interface IBookRepository
    {   
        // Listar - Paginação
        Task<(IEnumerable<Book> Books, int TotalCount)> GetBooksFromUserAsync(int userId, int pageNumber, int pageSize);
        // Cadastrar
        Task AddBookAsync(Book book);
        // Consultar um registro específico
        Task<Book?> FindByIdAsync(int id, int userId);
        Task<Book?> FindByTitleAsync(string title, int userId);
        // Editar um registro
        Task UpdateBookAsync(Book book);
        // Remover um registro
        Task DeleteBookAsync(Book book);

    }
}