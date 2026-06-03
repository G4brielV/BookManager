using BookManager.API.DTOs;
using BookManager.API.Models;

namespace BookManager.API.Mappings;

public static class UserMapper
{

    public static User ToEntity(this RegisterRequest request, string password)
    {
        return new User
        {
            Name = request.Name,
            Email = request.Email,
            Password = password
        };
    }

    public static RegisterResponse ToResponse(this User user)
    {
        return new RegisterResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };
    }
}