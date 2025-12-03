using Challenge.Viceri.Superhero.Application.Interfaces;
using Challenge.Viceri.Superhero.Infrastructure.Persistences.Data;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<SuperheroContext>(options => options.UseInMemoryDatabase($"SuperHeroDb"));
        services.AddScoped<ISuperheroContext>(provider => provider.GetRequiredService<SuperheroContext>());

        return services;
    }

    public static void InitializeDatabase(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SuperheroContext>();

        // EnsureCreated() cria o schema e aplica HasData()
        context.Database.EnsureCreated();
    }
}