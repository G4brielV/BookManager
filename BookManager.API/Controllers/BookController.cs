using System.Security.Claims;
using BookManager.API.DTOs;
using BookManager.API.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookController(IBookService bookService, IValidator<BookRequest> validator) : ControllerBase
{
    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<BookResponse>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        int userId = GetUserId();
        var result = await bookService.GetBooksAsync(userId, pageNumber, pageSize);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookResponse>> GetById(int id)
    {
        int userId = GetUserId();
        var result = await bookService.FindByIdAsync(id, userId);

        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<ActionResult<BookResponse>> Create([FromBody] BookRequest request)
    {
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));
        }

        int userId = GetUserId();
        var result = await bookService.CreateBookAsync(userId, request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BookResponse>> Update(int id, [FromBody] BookRequest request)
    {
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));
        }

        int userId = GetUserId();
        var result = await bookService.UpdateBookAsync(id, request, userId);

        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        int userId = GetUserId();
        var result = await bookService.DeleteBookAsync(id, userId);

        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }

        return NoContent();
    }
}
