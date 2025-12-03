using Challenge.Viceri.Superhero.Application.UseCases.SuperPowers.DataTransferObjects;
using Challenge.Viceri.Superhero.Domain.Entities;

namespace Challenge.Viceri.Superhero.Application.UseCases.Heros.DataTransferObjects;

public sealed class HeroDto
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public string Codename { get; init; } = default!;
    public float Height { get; init; }
    public float Weight { get; init; }
    public DateTime? DateBirth { get; init; }
    public List<SuperPowerDto?> SuperPower { get; init; } = default!;

    public static implicit operator HeroDto?(Hero? hero)
    {
        if (hero is null)
            return null;

        return new HeroDto
        {
            Id = hero.Id,
            Name = hero.Name,
            Codename = hero.Codename,
            DateBirth = hero.DateBirth,
            Height = hero.Height,
            Weight = hero.Weight,
            SuperPower = hero.HeroSuperPowers
                .Where(hsp => hsp.SuperPower != null)
                .Select(hsp => (SuperPowerDto?)hsp.SuperPower)
                .ToList()
        };
    }
}