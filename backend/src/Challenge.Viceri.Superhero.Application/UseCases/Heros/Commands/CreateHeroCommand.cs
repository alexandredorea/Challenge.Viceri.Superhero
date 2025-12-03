using Challenge.Viceri.Superhero.Application.Interfaces;
using Challenge.Viceri.Superhero.Application.UseCases.Heros.DataTransferObjects;
using Challenge.Viceri.Superhero.Domain.Entities;
using MediatR;

namespace Challenge.Viceri.Superhero.Application.UseCases.Heros.Commands;

public record CreateHeroCommand(
    string Name,
    string Codename,
    List<int> SuperpowerIds,
    DateTime? DateBirth,
    float Height,
    float Weight
) : IRequest<HeroDto>;

internal sealed class CreateHeroCommandHandler(ISuperheroContext context)
    : IRequestHandler<CreateHeroCommand, HeroDto>
{
    public async Task<HeroDto> Handle(CreateHeroCommand request, CancellationToken cancellationToken)
    {
        var newHero = Hero.Create(request.Name, request.Codename, request.Height, request.Weight, request.DateBirth);
        newHero.AddSuperPowers(request.SuperpowerIds);

        context.Heroes.Add(newHero);
        await context.SaveChangesAsync(cancellationToken);

        return newHero!;
    }
}