using Challenge.Viceri.Superhero.Application.Interfaces;
using Challenge.Viceri.Superhero.Application.UseCases.SuperPowers.DataTransferObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Challenge.Viceri.Superhero.Application.UseCases.SuperPowers.Queries;

public sealed record GetAllSuperPowerQuery : IRequest<IList<SuperPowerDto>>;

internal sealed class GetAllSuperPowerQueryHandler(ISuperheroContext context)
    : IRequestHandler<GetAllSuperPowerQuery, IList<SuperPowerDto>>
{
    public async Task<IList<SuperPowerDto>> Handle(GetAllSuperPowerQuery request, CancellationToken cancellationToken)
    {
        var heroes = await context.SuperPowers
            .ToListAsync(cancellationToken);

        if (heroes.Count == 0)
            return [];

        return heroes.ConvertAll(x => (SuperPowerDto)x!);
    }
}