using Challenge.Viceri.Superhero.Application.UseCases.Heros.Commands;
using FluentValidation;

namespace Challenge.Viceri.Superhero.Application.UseCases.Heros.Validations;

internal sealed class DeleteHeroCommandValidator : AbstractValidator<DeleteHeroCommand>
{
    public DeleteHeroCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("O id deve ser maior que zero.")
            .WithErrorCode("Hero.Id.Invalid");
    }
}