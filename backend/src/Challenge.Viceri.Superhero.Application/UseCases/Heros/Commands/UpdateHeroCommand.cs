using System.Text.Json.Serialization;
using Challenge.Viceri.Superhero.Application.Interfaces;
using Challenge.Viceri.Superhero.Application.UseCases.Heros.DataTransferObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Challenge.Viceri.Superhero.Application.UseCases.Heros.Commands;

public sealed class UpdateHeroCommand : IRequest<HeroDto?>
{
    [JsonIgnore]
    public int Id { get; private set; }

    public string Name { get; init; } = default!;

    public string Codename { get; init; } = default!;

    public List<int> SuperpowerIds { get; init; } = default!;

    public DateTime? DateBirth { get; init; }

    public float Height { get; init; }

    public float Weight { get; init; }

    public void SetHeroId(int id) => Id = id;
}

internal sealed class UpdateHeroCommandHandler(ISuperheroContext context)
        : IRequestHandler<UpdateHeroCommand, HeroDto?>
{
    public async Task<HeroDto?> Handle(UpdateHeroCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Heroes
            .Include(h => h.HeroSuperPowers)
            .FirstOrDefaultAsync(h => h.Id == request.Id, cancellationToken);

        if (entity is null)
            return null;

        entity.UpdateHero(request.Name, request.Codename, request.Height, request.Weight, request.DateBirth);
        entity.UpdateSuperPowers(request.SuperpowerIds);
        await context.SaveChangesAsync(cancellationToken);

        return entity;
    }
}