using BookManager.API.DTOs;
using BookManager.API.Services;
using BookManager.API.Services.Impl;
using BookManager.API.Repositories;
using NSubstitute;
using FluentAssertions;

namespace BookManager.Tests.Services;

public class AuthServiceTests
{
    private readonly IUserRepository _userRepositoryMock;
    private readonly ITokenService _tokenServiceMock;
    private readonly IAuthService _sut; 

    public AuthServiceTests()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _tokenServiceMock = Substitute.For<ITokenService>();
        _sut = new AuthService(_userRepositoryMock, _tokenServiceMock);
    }

    [Fact] 
    public async Task RegisterAsync_DeveRetornarFalha_QuandoEmailJaExiste()
    {
        // Prep
        var request = new RegisterRequest { Name = "Teste", Email = "teste@teste.com", Password = "123456" };
        _userRepositoryMock.EmailExistsAsync(request.Email).Returns(true); 

        // Act 
        var result = await _sut.RegisterAsync(request);

        // Assert 
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Este e-mail já está em uso.");
    }
}