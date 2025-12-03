using Challenge.Viceri.Superhero.Application.Interfaces;
using Challenge.Viceri.Superhero.Application.UseCases.Heros.DataTransferObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Challenge.Viceri.Superhero.Application.UseCases.Heros.Queries;

public record GetAllHeroesQuery : IRequest<IList<HeroDto>>;

internal sealed class GetAllHeroesQueryHandler(ISuperheroContext context)
    : IRequestHandler<GetAllHeroesQuery, IList<HeroDto>>
{
    public async Task<IList<HeroDto>> Handle(GetAllHeroesQuery request, CancellationToken cancellationToken)
    {
        var heroes = await context.Heroes
            .Include(h => h.HeroSuperPowers)
            .ThenInclude(hsp => hsp.SuperPower)
            .ToListAsync(cancellationToken);

        if (heroes.Count == 0)
            return [];

        return heroes.ConvertAll(x => (HeroDto)x!);
    }
}