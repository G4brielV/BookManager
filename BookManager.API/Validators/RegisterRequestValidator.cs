using BookManager.API.DTOs;
using FluentValidation;

namespace BookManager.API.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .Length(3, 50).WithMessage("O nome deve ter entre 3 e 50 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("O formato do e-mail é inválido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(6).WithMessage("A senha deve ter pelo menos 6 caracteres.")
            // Exemplo de validação complexa: obrigar a ter um número
            .Matches(@"[0-9]+").WithMessage("A senha deve conter pelo menos um número."); 
    }
}