using Challenge.Viceri.Superhero.Application.UseCases.SuperPowers.Commands;
using FluentValidation;

namespace Challenge.Viceri.Superhero.Application.UseCases.SuperPowers.Validators;

public sealed class DeleteSuperPowerCommandValidator : AbstractValidator<DeleteSuperPowerCommand>
{
    public DeleteSuperPowerCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("O id deve ser maior que zero.");
    }
}