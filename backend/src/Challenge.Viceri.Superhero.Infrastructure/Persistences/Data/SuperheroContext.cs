using Challenge.Viceri.Superhero.Application.Interfaces;
using Challenge.Viceri.Superhero.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Challenge.Viceri.Superhero.Infrastructure.Persistences.Data;

internal sealed class SuperheroContext(DbContextOptions<SuperheroContext> options)
    : DbContext(options), ISuperheroContext
{
    public DbSet<Hero> Heroes => Set<Hero>();
    public DbSet<SuperPower> SuperPowers => Set<SuperPower>();
    public DbSet<HeroSuperPower> HeroesSuperPowers => Set<HeroSuperPower>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SuperheroContext).Assembly);
        base.OnModelCreating(modelBuilder);

        // Seed Inicial para Superpoderes
        modelBuilder.Entity<SuperPower>()
            .HasData(
                SuperPower.Create("Super Força", "Força sobre-humana") with { Id = 1 },
                SuperPower.Create("Voo", "Capacidade de voar") with { Id = 2 },
                SuperPower.Create("Invulnerabilidade", "Resistência a danos físicos") with { Id = 3 },
                SuperPower.Create("Velocidade", "Velocidade sobre-humana") with { Id = 4 },
                SuperPower.Create("Telepatia", "Leitura e comunicação mental") with { Id = 5 }
            );
    }
}