using Challenge.Viceri.Superhero.Application.UseCases.SuperPowers.Commands;
using FluentValidation;

namespace Challenge.Viceri.Superhero.Application.UseCases.SuperPowers.Validators;

public sealed class CreateSuperPowerCommandValidator : AbstractValidator<CreateSuperPowerCommand>
{
    private const int MaxNameLength = 50;
    private const int MaxCodenameLength = 250;

    public CreateSuperPowerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(MaxNameLength).WithMessage("O nome não pode exceder {MaxLength} caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(MaxCodenameLength).WithMessage("A descrição não pode exceder {MaxLength} caracteres.");
    }
}