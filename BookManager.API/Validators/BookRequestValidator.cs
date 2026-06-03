using BookManager.API.DTOs;
using FluentValidation;

namespace BookManager.API.Validators;

public class BookRequestValidator : AbstractValidator<BookRequest>
{
    public BookRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("O título do livro é obrigatório.")
            .MaximumLength(100).WithMessage("O título deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("O nome do autor é obrigatório.")
            .MaximumLength(50).WithMessage("O autor deve ter no máximo 50 caracteres.");

        RuleFor(x => x.PublishDate)
            .NotEmpty().WithMessage("A data de publicação é obrigatória.")
            .LessThanOrEqualTo(DateTime.Now).WithMessage("A data de publicação não pode ser no futuro.");
    }
}