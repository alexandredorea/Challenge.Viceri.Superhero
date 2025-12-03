using Challenge.Viceri.Superhero.Application.Interfaces;
using Challenge.Viceri.Superhero.Application.UseCases.Heros.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Challenge.Viceri.Superhero.Application.UseCases.Heros.Validations;

internal sealed class UpdateHeroCommandValidatorDetalhado : AbstractValidator<UpdateHeroCommand>
{
    private readonly ISuperheroContext _context;

    private const int MaxNameLength = 120;
    private const int MaxCodenameLength = 120;
    private const float MinHeight = 0.5f;
    private const float MaxHeight = 3.0f;
    private const float MinWeight = 1.0f;
    private const float MaxWeight = 500.0f;
    private const int MinAgeYears = 15;
    private const int MaxAgeYears = 150;

    public UpdateHeroCommandValidatorDetalhado(ISuperheroContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));

        ConfigureSynchronousValidation();
        ConfigureAsynchronousValidation();
    }

    private void ConfigureSynchronousValidation()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("O Id deve ser maior que zero.")
            .WithErrorCode("Hero.Id.Invalid");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("O nome é obrigatório.")
            .WithErrorCode("Hero.Name.Required")
            .MaximumLength(MaxNameLength)
            .WithMessage("O nome não pode exceder {MaxLength} caracteres.")
            .WithErrorCode("Hero.Name.MaxLength");

        RuleFor(x => x.Codename)
            .NotEmpty()
            .WithMessage("O alter ego ({PropertyName}) é obrigatório.")
            .WithErrorCode("Hero.Codename.Required")
            .MaximumLength(MaxCodenameLength)
            .WithMessage("O alter ego ({PropertyName}) não pode exceder {MaxLength} caracteres.")
            .WithErrorCode("Hero.Codename.MaxLength");

        RuleFor(x => x.SuperpowerIds)
            .NotNull()
            .WithMessage("A lista de Superpoderes não pode ser nula.")
            .WithErrorCode("Hero.Superpowers.Required")
            .NotEmpty()
            .WithMessage("A lista de Superpoderes deve conter ao menos um item.")
            .WithErrorCode("Hero.Superpowers.Empty")
            .Must(ids => ids.All(id => id > 0))
            .WithMessage("Todos os IDs de Superpoderes devem ser maiores que zero.")
            .WithErrorCode("Hero.Superpowers.InvalidIds")
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("A lista de Superpoderes contém IDs duplicados.")
            .WithErrorCode("Hero.Superpowers.Duplicates");

        RuleFor(x => x.Height)
            .GreaterThan(0)
            .WithMessage("A altura deve ser maior que zero.")
            .WithErrorCode("Hero.Height.Invalid")
            .LessThan(MaxHeight)
            .WithMessage("A altura não pode ser maior que {ComparisonValue}m.")
            .WithErrorCode("Hero.Height.MaxValue");

        RuleFor(x => x.Weight)
            .GreaterThan(0)
            .WithMessage("O peso deve ser maior que zero.")
            .WithErrorCode("Hero.Weight.Invalid")
            .LessThan(MaxWeight)
            .WithMessage("O peso não pode ser maior que {ComparisonValue} kg.")
            .WithErrorCode("Hero.Weight.MaxValue");

        When(x => x.DateBirth.HasValue, () =>
        {
            RuleFor(x => x.DateBirth!.Value)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("A data de nascimento não pode ser no futuro.")
                .WithErrorCode("Hero.DateBirth.Future");

            RuleFor(x => x.DateBirth!.Value)
                .GreaterThanOrEqualTo(DateTime.UtcNow.AddYears(-MaxAgeYears))
                .WithMessage($"A data de nascimento não pode ser anterior a {MaxAgeYears} anos atrás.")
                .WithErrorCode("Hero.DateBirth.TooOld");

            // Validação de idade mínima
            RuleFor(x => x.DateBirth!.Value)
                .LessThanOrEqualTo(DateTime.UtcNow.AddYears(-MinAgeYears))
                .WithMessage($"O herói deve ter pelo menos {MinAgeYears} anos.")
                .WithErrorCode("Hero.DateBirth.TooYoung")
                .When(x => MinAgeYears > 0);

            // Validação de data razoável (não aceitar datas muito antigas)
            RuleFor(x => x.DateBirth!.Value)
                .GreaterThan(new DateTime(1800, 1, 1))
                .WithMessage("A data de nascimento deve ser posterior a 01/01/1800.")
                .WithErrorCode("Hero.DateBirth.TooAncient");

            // Validação customizada para calcular idade
            RuleFor(x => x.DateBirth!.Value)
                .Must(BeAReasonableAge)
                .WithMessage(x => $"A idade de {CalculateAge(x.DateBirth!.Value)} anos não é realista.")
                .WithErrorCode("Hero.DateBirth.UnreasonableAge");
        });
    }

    private void ConfigureAsynchronousValidation()
    {
        RuleFor(x => x.Id)
            .MustAsync(HeroExistsAsync)
            .WithMessage(x => $"Herói com Id '{x.Id}' não foi encontrado.")
            .WithErrorCode("Hero.NotFound");

        RuleFor(x => x)
            .MustAsync(CodenameIsUniqueAsync)
            .WithMessage(x => $"Já existe outro herói com o nome '{x.Codename}'.")
            .WithErrorCode("Hero.Codename.Conflict")
            .WithName("Codename");

        RuleFor(x => x.SuperpowerIds)
            .MustAsync(AllSuperpowersExistAsync)
            .WithMessage(ValidateSuperpowersWithDetails)
            .WithErrorCode("Hero.Superpowers.Invalid");
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

    private async Task<bool> HeroExistsAsync(int heroId, CancellationToken cancellationToken)
    {
        return await _context.Heroes
            .AnyAsync(h => h.Id == heroId, cancellationToken);
    }

    private async Task<bool> CodenameIsUniqueAsync(
        UpdateHeroCommand command,
        CancellationToken cancellationToken)
    {
        var existsAnotherHero = await _context.Heroes
            .AnyAsync(h => h.Codename == command.Codename && h.Id != command.Id, cancellationToken);

        return !existsAnotherHero;
    }

    private async Task<bool> AllSuperpowersExistAsync(
        IEnumerable<int> superpoderesIds,
        CancellationToken cancellationToken)
    {
        if (superpoderesIds == null || !superpoderesIds.Any())
            return false;

        var distinctIds = superpoderesIds.Distinct().ToList();

        var existingSuperpowers = await _context.SuperPowers
            .Where(sp => distinctIds.Contains(sp.Id))
            .Select(sp => sp.Id)
            .ToListAsync(cancellationToken);

        return existingSuperpowers.Count == distinctIds.Count;
    }

    private string ValidateSuperpowersWithDetails(UpdateHeroCommand command)
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
}