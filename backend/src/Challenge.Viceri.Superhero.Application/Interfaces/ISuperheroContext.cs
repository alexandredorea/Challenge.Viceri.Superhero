using Challenge.Viceri.Superhero.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Challenge.Viceri.Superhero.Application.Interfaces;

public interface ISuperheroContext
{
    DbSet<Hero> Heroes { get; }

    DbSet<SuperPower> SuperPowers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}