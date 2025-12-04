using Challenge.Viceri.Superhero.Infrastructure.Persistences.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Challenge.Viceri.Superhero.Test.Integrations.Setup;

public sealed class SuperheroWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        //builder.ConfigureTestServices(services =>
        //{
        //    // Remove o DbContext existente
        //    services.RemoveAll(typeof(DbContextOptions<SuperheroContext>));
        //    services.RemoveAll(typeof(SuperheroContext));
        //    services.RemoveAll(typeof(ISuperheroContext));

        //    // Adiciona DbContext com InMemory Database
        //    services.AddDbContext<SuperheroContext>(options =>
        //    {
        //        options.UseInMemoryDatabase("SuperheroTestDb");
        //    });

        //    services.AddScoped<ISuperheroContext>(provider =>
        //        provider.GetRequiredService<SuperheroContext>());

        //    // Cria o banco e aplica seed se necessário
        //    var serviceProvider = services.BuildServiceProvider();
        //    using var scope = serviceProvider.CreateScope();
        //    var context = scope.ServiceProvider.GetRequiredService<SuperheroContext>();
        //    context.Database.EnsureCreated();
        //});

        //builder.UseEnvironment("Testing");
    }

    public async Task<T> ExecuteDbContextAsync<T>(Func<SuperheroContext, Task<T>> action)
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SuperheroContext>();
        return await action(context);
    }

    public async Task ExecuteDbContextAsync(Func<SuperheroContext, Task> action)
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SuperheroContext>();
        await action(context);
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SuperheroContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}