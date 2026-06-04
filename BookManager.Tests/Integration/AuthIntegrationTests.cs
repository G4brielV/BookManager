using System.Net;
using System.Net.Http.Json;
using BookManager.API.DTOs;
using BookManager.API.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace BookManager.Tests.Integration;

public class AuthIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    public AuthIntegrationTests()
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

    [Fact]
    public async Task Register_Endpoint_DeveRetornar201_ComDadosValidos()
    {
        // Prep
        var request = new RegisterRequest 
        { 
            Name = "Integração", 
            Email = "novo@teste.com", 
            Password = "senhaForte123" 
        };

        // Act 
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}