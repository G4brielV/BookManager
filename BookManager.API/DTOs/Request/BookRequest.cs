namespace BookManager.API.DTOs;

public class BookRequest
{
    public required string Title { get; set; }
    public required string Author { get; set; }
    public required DateTime PublishDate { get; set; }
}