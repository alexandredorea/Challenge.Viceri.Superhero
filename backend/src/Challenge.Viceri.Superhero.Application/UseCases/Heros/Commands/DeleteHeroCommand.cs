using Challenge.Viceri.Superhero.Application.Interfaces;
using Challenge.Viceri.Superhero.Application.UseCases.Heros.DataTransferObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Challenge.Viceri.Superhero.Application.UseCases.Heros.Commands;

public record DeleteHeroCommand(int Id) : IRequest<HeroDto?>;

internal sealed class DeleteHeroCommandHandler(ISuperheroContext context) : IRequestHandler<DeleteHeroCommand, HeroDto?>
{
    public async Task<HeroDto?> Handle(DeleteHeroCommand request, CancellationToken cancellationToken)
    {
        var hero = await context.Heroes.FirstOrDefaultAsync(h => h.Id == request.Id, cancellationToken);
        if (hero is not null)
        {
            context.Heroes.Remove(hero);
            await context.SaveChangesAsync(cancellationToken);
        }
        return hero;
    }
}