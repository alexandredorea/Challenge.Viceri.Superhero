using System.Net;
using System.Net.Http.Json;
using Challenge.Viceri.Superhero.Api.Commons;
using Challenge.Viceri.Superhero.Application.UseCases.Heros.Commands;
using Challenge.Viceri.Superhero.Application.UseCases.Heros.DataTransferObjects;
using Challenge.Viceri.Superhero.Domain.Entities;
using Challenge.Viceri.Superhero.Test.Integrations.Setup;
using FluentAssertions;

namespace Challenge.Viceri.Superhero.Test.Integrations.Controllers;

public sealed class HeroesControllerIntegrationTests : IClassFixture<SuperheroWebApplicationFactory>, IAsyncLifetime
{
    private readonly SuperheroWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public HeroesControllerIntegrationTests(SuperheroWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    [Fact]
    public async Task GetAllHeroes_WithNoHeroes_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/heroes");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllHeroes_WithExistingHeroes_ShouldReturnHeroes()
    {
        // Arrange
        await SeedHeroesAsync();

        // Act
        var response = await _client.GetAsync("/api/heroes");
        var result = await response.Content.ReadFromJsonAsync<ApiResult<IEnumerable<HeroDto>>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
        result.Data.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetHeroById_WithValidId_ShouldReturnHero()
    {
        // Arrange
        var heroId = await SeedSingleHeroAsync();

        // Act
        var response = await _client.GetAsync($"/api/heroes/{heroId}");
        var result = await response.Content.ReadFromJsonAsync<ApiResult<HeroDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(heroId);
        result.Data.Name.Should().Be("Peter Parker");
    }

    [Fact]
    public async Task GetHeroById_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/heroes/9999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateHero_WithValidCommand_ShouldCreateHero()
    {
        // Arrange
        var superPowerId = await SeedSuperPowerAsync();
        var command = new CreateHeroCommand(
            Name: "Bruce Wayne",
            Codename: "Batman",
            SuperpowerIds: new List<int> { superPowerId },
            DateBirth: new DateTime(1980, 2, 19),
            Height: 1.88f,
            Weight: 95.0f
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/heroes", command);
        var result = await response.Content.ReadFromJsonAsync<ApiResult<HeroDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be(command.Name);
        result.Data.Codename.Should().Be(command.Codename);
        result.Data.SuperPowers.Should().ContainSingle();
    }

    [Fact]
    public async Task CreateHero_WithInvalidCommand_ShouldReturnBadRequest()
    {
        // Arrange
        var command = new CreateHeroCommand(
            Name: "",
            Codename: "Batman",
            SuperpowerIds: new List<int>(),
            DateBirth: null,
            Height: 1.88f,
            Weight: 95.0f
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/heroes", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateHero_WithValidCommand_ShouldUpdateHero()
    {
        // Arrange
        var heroId = await SeedSingleHeroAsync();
        var superPowerId = await SeedSuperPowerAsync();

        var command = new UpdateHeroCommand
        {
            Name = "Peter Benjamin Parker",
            Codename = "The Amazing Homem-Aranha",
            SuperpowerIds = new List<int> { superPowerId },
            DateBirth = new DateTime(1995, 8, 10),
            Height = 1.80f,
            Weight = 78.0f
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/heroes/{heroId}", command);
        var result = await response.Content.ReadFromJsonAsync<ApiResult<HeroDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data!.Name.Should().Be(command.Name);
        result.Data.Codename.Should().Be(command.Codename);
    }

    [Fact]
    public async Task UpdateHero_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var command = new UpdateHeroCommand
        {
            Name = "Test",
            Codename = "Test",
            SuperpowerIds = new List<int> { 1 },
            DateBirth = null,
            Height = 1.80f,
            Weight = 78.0f
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/heroes/9999", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteHero_WithValidId_ShouldDeleteHero()
    {
        // Arrange
        var heroId = await SeedSingleHeroAsync();

        // Act
        var response = await _client.DeleteAsync($"/api/heroes/{heroId}");
        var result = await response.Content.ReadFromJsonAsync<ApiResult<HeroDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();

        // Verify hero is deleted
        var getResponse = await _client.GetAsync($"/api/heroes/{heroId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteHero_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/heroes/9999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateHero_WithDuplicateCodename_ShouldReturnBadRequest()
    {
        // Arrange
        var heroId = await SeedSingleHeroAsync();
        var superPowerId = await SeedSuperPowerAsync();

        var command = new CreateHeroCommand(
            Name: "Miles Morales",
            Codename: "Homem-Aranha", // Duplica
            SuperpowerIds: new List<int> { superPowerId },
            DateBirth: new DateTime(2000, 8, 10),
            Height: 1.73f,
            Weight: 72.0f
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/heroes", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAndGetHero_EndToEndFlow_ShouldWork()
    {
        // Arrange
        var superPowerId = await SeedSuperPowerAsync();
        var createCommand = new CreateHeroCommand(
            Name: "Tony Stark",
            Codename: "Homem de Ferro",
            SuperpowerIds: new List<int> { superPowerId },
            DateBirth: new DateTime(1970, 5, 29),
            Height: 1.85f,
            Weight: 102.0f
        );

        // Act - Create
        var createResponse = await _client.PostAsJsonAsync("/api/heroes", createCommand);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResult<HeroDto>>();

        // Act - Get
        var getResponse = await _client.GetAsync($"/api/heroes/{createResult!.Data!.Id}");
        var getResult = await getResponse.Content.ReadFromJsonAsync<ApiResult<HeroDto>>();

        // Assert
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        getResult!.Data!.Name.Should().Be(createCommand.Name);
        getResult.Data.Codename.Should().Be(createCommand.Codename);
        getResult.Data.Height.Should().Be(createCommand.Height);
        getResult.Data.Weight.Should().Be(createCommand.Weight);
    }

    // Helpers
    private async Task<int> SeedSuperPowerAsync()
    {
        return await _factory.ExecuteDbContextAsync(async context =>
        {
            var superPower = SuperPower.Create("Superforça", "Força incrível");
            context.SuperPowers.Add(superPower);
            await context.SaveChangesAsync();
            return superPower.Id;
        });
    }

    private async Task<int> SeedSingleHeroAsync()
    {
        return await _factory.ExecuteDbContextAsync(async context =>
        {
            var superPower = SuperPower.Create("Lançamento de Teias", "Pode atirar teias");
            context.SuperPowers.Add(superPower);
            await context.SaveChangesAsync();

            var hero = Hero.Create("Peter Parker", "Homem-Aranha", 1.78f, 76.5f, new DateTime(1995, 8, 10));
            hero.AddSuperPowers(new List<int> { superPower.Id });
            context.Heroes.Add(hero);
            await context.SaveChangesAsync();
            return hero.Id;
        });
    }

    private async Task SeedHeroesAsync()
    {
        await _factory.ExecuteDbContextAsync(async context =>
        {
            var power1 = SuperPower.Create("Voo", "Pode voar");
            var power2 = SuperPower.Create("Supervelocidade", "Velocidade incrível");
            context.SuperPowers.AddRange(power1, power2);
            await context.SaveChangesAsync();

            var hero1 = Hero.Create("Clark Kent", "Superman", 1.90f, 107.0f);
            hero1.AddSuperPowers(new List<int> { power1.Id, power2.Id });

            var hero2 = Hero.Create("Diana Prince", "Mulher Maravilha", 1.83f, 75.0f);
            hero2.AddSuperPowers(new List<int> { power1.Id });

            context.Heroes.AddRange(hero1, hero2);
            await context.SaveChangesAsync();
        });
    }
}