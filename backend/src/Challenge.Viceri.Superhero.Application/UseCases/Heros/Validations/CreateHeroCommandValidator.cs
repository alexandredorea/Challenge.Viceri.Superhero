using Challenge.Viceri.Superhero.Application.Interfaces;
using Challenge.Viceri.Superhero.Application.UseCases.Heros.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Challenge.Viceri.Superhero.Application.UseCases.Heros.Validations;

public sealed class CreateHeroCommandValidator : AbstractValidator<CreateHeroCommand>
{
    private readonly ISuperheroContext _context;

    private const int MaxNameLength = 120;
    private const int MaxCodenameLength = 120;
    private const float MaxHeight = 3.0f;
    private const float MaxWeight = 500.0f;
    private const int MinAgeYears = 15;
    private const int MaxAgeYears = 150;

    public CreateHeroCommandValidator(ISuperheroContext context)
    {
        _context = context;

        ConfigureSynchronousValidation();
        ConfigureAsynchronousValidation();
    }

    private void ConfigureSynchronousValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(MaxNameLength).WithMessage("O nome não pode exceder {MaxLength} caracteres.");

        RuleFor(x => x.Codename)
            .NotEmpty().WithMessage("O alter ego ({PropertyName}) é obrigatório.")
            .MaximumLength(MaxCodenameLength).WithMessage("O alter ego ({PropertyName}) não pode exceder {MaxLength} caracteres.");

        RuleFor(x => x.SuperpowerIds)
            .NotNull().WithMessage("A lista de Superpoderes não pode ser nula.")
            .NotEmpty().WithMessage("A lista de Superpoderes deve conter ao menos um item.")
            .Must(ids => ids.All(id => id > 0)).WithMessage("Todos os IDs de Superpoderes devem ser maiores que zero.")
            .Must(ids => ids.Distinct().Count() == ids.Count).WithMessage("A lista de Superpoderes contém IDs duplicados.");

        RuleFor(x => x.Height)
            .GreaterThan(0).WithMessage("A altura deve ser maior que zero.")
            .LessThan(MaxHeight).WithMessage("A altura não pode ser maior que {ComparisonValue}m.");

        RuleFor(x => x.Weight)
            .GreaterThan(0)
            .WithMessage("O peso deve ser maior que zero.")
            .LessThan(MaxWeight)
            .WithMessage("O peso não pode ser maior que {ComparisonValue} kg.");

        When(x => x.DateBirth.HasValue, () =>
        {
            RuleFor(x => x.DateBirth!.Value)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("A data de nascimento não pode ser no futuro.")
                .GreaterThanOrEqualTo(DateTime.UtcNow.AddYears(-MaxAgeYears)).WithMessage($"A data de nascimento não pode ser anterior a {MaxAgeYears} anos atrás.")
                .LessThanOrEqualTo(DateTime.UtcNow.AddYears(-MinAgeYears)).WithMessage($"O herói deve ter pelo menos {MinAgeYears} anos.")
                .When(x => MinAgeYears > 0).GreaterThan(new DateTime(1800, 1, 1)).WithMessage("A data de nascimento deve ser posterior a 01/01/1800.")
                .Must(BeAReasonableAge).WithMessage(x => $"A idade de {CalculateAge(x.DateBirth!.Value)} anos não é realista.");
        });
    }

    private static bool BeAReasonableAge(DateTime dateBirth)
    {
        var age = CalculateAge(dateBirth);
        return age >= MinAgeYears && age <= MaxAgeYears;
    }

    private static int CalculateAge(DateTime dateBirth)
    {
        var today = DateTime.UtcNow;
        var age = today.Year - dateBirth.Year;

        if (dateBirth.Date > today.AddYears(-age))
            age--;

        return age;
    }

    private void ConfigureAsynchronousValidation()
    {
        RuleFor(x => x.Codename)
            .MustAsync(BeUniqueCodenameAsync).WithMessage(x => $"Já existe outro herói com o nome '{x.Codename}'.");

        RuleFor(x => x.SuperpowerIds)
            .MustAsync(AllSuperpowersExistAsync).WithMessage(ValidateSuperpowersWithDetails);
    }

    private string ValidateSuperpowersWithDetails(CreateHeroCommand command)
    {
        var distinctIds = command.SuperpowerIds.Distinct().ToList();
        var inaa = _context.SuperPowers
                .Where(sp => distinctIds.Contains(sp.Id))
                .Select(sp => sp.Id);

        var invalidIds = distinctIds.Except(inaa).ToList();

        if (invalidIds.Count != 0)
        {
            return $"Os seguintes IDs de Superpoderes não existem: {string.Join(", ", invalidIds)}.";
        }

        return "Um ou mais Superpoderes informados não são válidos.";
    }

    private async Task<bool> BeUniqueCodenameAsync(string codename, CancellationToken cancellationToken)
    {
        var exists = await _context.Heroes
            .AnyAsync(h => h.Codename == codename, cancellationToken);

        return !exists;
    }

    private async Task<bool> AllSuperpowersExistAsync(
        IList<int> superpowerIds,
        CancellationToken cancellationToken)
    {
        if (superpowerIds is null || superpowerIds.Count == 0)
            return false;

        var count = await _context.SuperPowers
            .CountAsync(sp => superpowerIds.Contains(sp.Id), cancellationToken);

        return count == superpowerIds.Count;
    }
}