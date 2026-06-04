using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BookManager.API.Data;
using BookManager.API.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace BookManager.Tests.Integration;

public class BookIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public BookIntegrationTests()
    {
        _dbContainer = new PostgreSqlBuilder("postgres:15-alpine")
            .WithDatabase("book_manager_test")
            .WithUsername("admin")
            .WithPassword("senha123")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("ConnectionStrings:AppDbConnectionString", _dbContainer.GetConnectionString());
            });

        _client = _factory.CreateClient();

        // Aplica as migrations no banco de teste
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _dbContainer.DisposeAsync();
    }

  
    /// registra um usuário e faz login, retornando o token JWT.
    private async Task<string> RegisterAndLoginAsync(string name, string email, string password)
    {
        var registerRequest = new RegisterRequest
        {
            Name = name,
            Email = email,
            Password = password
        };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();

        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = password
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();

        var loginData = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions);
        return loginData!.Token;
    }

    // Cria um livro autenticado e retorna o response.
    private async Task<BookResponse> CreateBookAuthenticatedAsync(string token, BookRequest bookRequest)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/book");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(bookRequest);

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<BookResponse>(JsonOptions))!;
    }

    [Fact]
    public async Task GetBooks_SemToken_DeveRetornar401Unauthorized()
    {
        // Act 
        var response = await _client.GetAsync("/api/book");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetBooks_ComPaginacao_DeveRetornarApenasSegundoItem()
    {
        // Prep — registra, loga e cria 2 livros
        string token = await RegisterAndLoginAsync("User Paginação", "paginacao@teste.com", "senha123");
        await CreateBookAuthenticatedAsync(token, new BookRequest
        {
            Title = "Livro Alpha",
            Author = "Autor A",
            PublishDate = new DateOnly(2020, 1, 1)
        });
        await CreateBookAuthenticatedAsync(token, new BookRequest
        {
            Title = "Livro Beta",
            Author = "Autor B",
            PublishDate = new DateOnly(2021, 6, 15)
        });

        // Act — pede a página 2 com pageSize 1
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/book?pageNumber=2&pageSize=1");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<PaginatedResponse<BookResponse>>(JsonOptions);

        body.Should().NotBeNull();
        body!.TotalCount.Should().Be(2);
        body.PageNumber.Should().Be(2);
        body.PageSize.Should().Be(1);
        body.Items.Should().HaveCount(1);

        body.Items.First().Title.Should().Be("Livro Beta");
    }


    [Fact]
    public async Task DeleteBook_DeOutroUsuario_DeveRetornar404NotFound()
    {
        // Prep — cria Usuário A e seu livro
        string tokenA = await RegisterAndLoginAsync("User A", "userA@teste.com", "senha123");

        var livroDoA = await CreateBookAuthenticatedAsync(tokenA, new BookRequest
        {
            Title = "Livro do Usuário A",
            Author = "Autor A",
            PublishDate = new DateOnly(2022, 3, 10)
        });

        // cria Usuário B
        string tokenB = await RegisterAndLoginAsync("User B", "userB@teste.com", "senha456");

        // Act — Usuário B tenta deletar o livro do Usuário A
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/book/{livroDoA.Id}");
        deleteRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenB);
        var response = await _client.SendAsync(deleteRequest);

        // Assert — retorna 404 
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
