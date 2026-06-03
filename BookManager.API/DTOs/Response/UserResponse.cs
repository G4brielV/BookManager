namespace BookManager.API.DTOs
{
    public class UserResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
    }
}