using Challenge.Viceri.Superhero.Application.Interfaces;
using Challenge.Viceri.Superhero.Application.UseCases.Heros.DataTransferObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Challenge.Viceri.Superhero.Application.UseCases.Heros.Queries;

public record GetHeroByIdQuery(int Id) : IRequest<HeroDto?>;

internal sealed class GetHeroByIdQueryHandler(ISuperheroContext context)
    : IRequestHandler<GetHeroByIdQuery, HeroDto?>
{
    public async Task<HeroDto?> Handle(GetHeroByIdQuery request, CancellationToken cancellationToken)
    {
        var hero = await context.Heroes
            .Include(h => h.HeroSuperPowers)
            .ThenInclude(hsp => hsp.SuperPower)
            .Where(h => h.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return hero;
    }
}