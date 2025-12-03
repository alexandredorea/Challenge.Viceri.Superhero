using Challenge.Viceri.Superhero.Domain.Entities;

namespace Challenge.Viceri.Superhero.Application.UseCases.SuperPowers.DataTransferObjects;

public sealed record SuperPowerDto(
    int Id,
    string Name,
    string? Description)
{
    public static implicit operator SuperPowerDto?(SuperPower superPower)
    {
        if (superPower is null)
            return null;

        return new SuperPowerDto(
            Id: superPower.Id,
            Name: superPower.Name,
            Description: superPower.Description);
    }
}