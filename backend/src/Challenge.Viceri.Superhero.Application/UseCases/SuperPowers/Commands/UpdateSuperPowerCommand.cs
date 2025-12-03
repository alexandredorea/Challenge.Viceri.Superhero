using System.Text.Json.Serialization;
using Challenge.Viceri.Superhero.Application.Interfaces;
using Challenge.Viceri.Superhero.Application.UseCases.SuperPowers.DataTransferObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Challenge.Viceri.Superhero.Application.UseCases.SuperPowers.Commands;

public sealed class UpdateSuperPowerCommand : IRequest<SuperPowerDto?>
{
    [JsonIgnore]
    public int Id { get; private set; }

    public string Name { get; init; } = default!;

    public string? Description { get; init; }

    public void SetHeroId(int id) => Id = id;
}

internal sealed class UpdateSuperPowerCommandHandler(ISuperheroContext context)
        : IRequestHandler<UpdateSuperPowerCommand, SuperPowerDto?>
{
    public async Task<SuperPowerDto?> Handle(UpdateSuperPowerCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.SuperPowers.FirstOrDefaultAsync(h => h.Id == request.Id, cancellationToken);

        if (entity is null)
            return null;

        entity = entity with { Name = request.Name, Description = request.Description };
        await context.SaveChangesAsync(cancellationToken);

        return entity;
    }
}