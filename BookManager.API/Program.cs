using BookManager.API.Data;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using BookManager.API.Validators;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Validator
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

// DB 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AppDbConnectionString")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); 
}

app.UseAuthorization();

app.MapControllers();

app.Run();