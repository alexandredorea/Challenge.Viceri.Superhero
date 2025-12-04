using Challenge.Viceri.Superhero.Application.Interfaces;
using Challenge.Viceri.Superhero.Application.UseCases.SuperPowers.DataTransferObjects;
using Challenge.Viceri.Superhero.Domain.Entities;
using MediatR;

namespace Challenge.Viceri.Superhero.Application.UseCases.SuperPowers.Commands;

public sealed record CreateSuperPowerCommand(string Name, string? Description) : IRequest<SuperPowerDto>;

internal sealed class CreateSuperPowerCommandHandler(ISuperheroContext context)
    : IRequestHandler<CreateSuperPowerCommand, SuperPowerDto>
{
    public async Task<SuperPowerDto> Handle(CreateSuperPowerCommand request, CancellationToken cancellationToken)
    {
        var newSuperPower = SuperPower.Create(request.Name, request.Description);

        context.SuperPowers.Add(newSuperPower);
        await context.SaveChangesAsync(cancellationToken);

        return newSuperPower!;
    }
}