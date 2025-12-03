using Challenge.Viceri.Superhero.Application.Interfaces;
using Challenge.Viceri.Superhero.Application.UseCases.SuperPowers.DataTransferObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Challenge.Viceri.Superhero.Application.UseCases.SuperPowers.Commands;

public sealed record DeleteSuperPowerCommand(int Id) : IRequest<SuperPowerDto?>;

internal sealed class DeleteSuperPowerCommandHandler(ISuperheroContext context) : IRequestHandler<DeleteSuperPowerCommand, SuperPowerDto?>
{
    public async Task<SuperPowerDto?> Handle(DeleteSuperPowerCommand request, CancellationToken cancellationToken)
    {
        var superPower = await context.SuperPowers.FirstOrDefaultAsync(h => h.Id == request.Id, cancellationToken);
        if (superPower is not null)
        {
            context.SuperPowers.Remove(superPower);
            await context.SaveChangesAsync(cancellationToken);
        }
        return superPower!;
    }
}