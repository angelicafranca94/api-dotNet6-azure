using FluentValidation;
using Manager.Domain.Entities;

namespace Manager.Domain.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u)
            .NotEmpty()
            .WithMessage("A entidade não pode ser vazia!")

            .NotNull()
            .WithMessage("A entidade não pode ser nula!");

        RuleFor(u => u.Name)
            .NotNull()
            .WithMessage("Nome não pode ser nulo!")

            .NotEmpty()
            .WithMessage("Nome não pode ser vazio!")

            .MinimumLength(3)
            .WithMessage("Nome deve conter no mínimo 3 caracteres")

            .MaximumLength(50)
            .WithMessage("Nome deve conter no máximo 50 caracteres");

        RuleFor(u => u.Email)
            .NotNull()
            .WithMessage("Email não pode ser nulo!")

            .NotEmpty()
            .WithMessage("Email não pode ser vazio!")

            .MinimumLength(10)
            .WithMessage("Email deve conter no mínimo 10 caracteres")

            .MaximumLength(50)
            .WithMessage("Email deve conter no máximo 50 caracteres")

            .Matches(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$")
            .WithMessage("O email informado não é válido.");


        RuleFor(u => u.Password)
            .NotNull()
            .WithMessage("Senha não pode ser nulo!")

            .NotEmpty()
            .WithMessage("Senha não pode ser vazio!")

            .MinimumLength(6)
            .WithMessage("Senha deve conter no mínimo 6 caracteres")

            .MaximumLength(20)
            .WithMessage("Senha deve conter no máximo 20 caracteres"); ;
    }
}
