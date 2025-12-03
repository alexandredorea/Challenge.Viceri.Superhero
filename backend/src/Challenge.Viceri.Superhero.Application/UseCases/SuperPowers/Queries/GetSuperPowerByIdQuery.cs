using Challenge.Viceri.Superhero.Application.Interfaces;
using Challenge.Viceri.Superhero.Application.UseCases.SuperPowers.DataTransferObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Challenge.Viceri.Superhero.Application.UseCases.SuperPowers.Queries;

public sealed record GetSuperPowerByIdQuery(int Id) : IRequest<SuperPowerDto?>;

internal sealed class GetSuperPowerByIdQueryHandler(ISuperheroContext context)
    : IRequestHandler<GetSuperPowerByIdQuery, SuperPowerDto?>
{
    public async Task<SuperPowerDto?> Handle(GetSuperPowerByIdQuery request, CancellationToken cancellationToken)
    {
        var superPower = await context.SuperPowers
            .Where(h => h.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return superPower!;
    }
}