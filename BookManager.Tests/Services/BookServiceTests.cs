using BookManager.API.DTOs;
using BookManager.API.Models;
using BookManager.API.Repositories;
using BookManager.API.Services;
using BookManager.API.Services.Impl;
using FluentAssertions;
using NSubstitute;

namespace BookManager.Tests.Services;

public class BookServiceTests
{
    private readonly IBookRepository _bookRepositoryMock;
    private readonly IBookService _sut;

    public BookServiceTests()
    {
        _bookRepositoryMock = Substitute.For<IBookRepository>();
        _sut = new BookService(_bookRepositoryMock);
    }

    [Fact]
    public async Task CreateBookAsync_DeveRetornarFalha_QuandoTituloJaExisteParaUsuario()
    {
        // Prep
        int userId = 1;
        var request = new BookRequest
        {
            Title = "Clean Code",
            Author = "Robert C. Martin",
            PublishDate = new DateOnly(2008, 8, 1)
        };

        _bookRepositoryMock.FindByTitleAsync(request.Title, userId)
            .Returns(new Book
            {
                Id = 10,
                Title = "Clean Code",
                Author = "Robert C. Martin",
                PublishDate = new DateOnly(2008, 8, 1),
                UserId = userId
            });

        // Act
        var result = await _sut.CreateBookAsync(userId, request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Já existe um livro com este título cadastrado.");
    }
}
