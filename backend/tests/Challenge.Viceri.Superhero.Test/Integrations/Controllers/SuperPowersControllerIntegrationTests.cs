using System.Net;
using System.Net.Http.Json;
using Challenge.Viceri.Superhero.Api.Commons;
using Challenge.Viceri.Superhero.Application.UseCases.SuperPowers.Commands;
using Challenge.Viceri.Superhero.Application.UseCases.SuperPowers.DataTransferObjects;
using Challenge.Viceri.Superhero.Domain.Entities;
using Challenge.Viceri.Superhero.Test.Integrations.Setup;
using FluentAssertions;

namespace Challenge.Viceri.Superhero.Test.Integrations.Controllers;

public class SuperPowersControllerIntegrationTests : IClassFixture<SuperheroWebApplicationFactory>, IAsyncLifetime
{
    private readonly SuperheroWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public SuperPowersControllerIntegrationTests(SuperheroWebApplicationFactory factory)
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
    public async Task GetAllSuperPowers_WithNoSuperPowers_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/superpowers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllSuperPowers_WithExistingSuperPowers_ShouldReturnSuperPowers()
    {
        // Arrange
        await SeedSuperPowersAsync();

        // Act
        var response = await _client.GetAsync("/api/superpowers");
        var result = await response.Content.ReadFromJsonAsync<ApiResult<IEnumerable<SuperPowerDto>>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
        result.Data.Should().HaveCountGreaterThanOrEqualTo(3);
    }

    [Fact]
    public async Task GetSuperPowerById_WithValidId_ShouldReturnSuperPower()
    {
        // Arrange
        var superPowerId = await SeedSingleSuperPowerAsync();

        // Act
        var response = await _client.GetAsync($"/api/superpowers/{superPowerId}");
        var result = await response.Content.ReadFromJsonAsync<ApiResult<SuperPowerDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(superPowerId);
        result.Data.Name.Should().Be("Superforça");
    }

    [Fact]
    public async Task GetSuperPowerById_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/superpowers/9999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateSuperPower_WithValidCommand_ShouldCreateSuperPower()
    {
        // Arrange
        var command = new CreateSuperPowerCommand(
            Name: "Telepatia",
            Description: "Capacidade de ler mentes e se comunicar mentalmente"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/superpowers", command);
        var result = await response.Content.ReadFromJsonAsync<ApiResult<SuperPowerDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be(command.Name);
        result.Data.Description.Should().Be(command.Description);
    }

    [Fact]
    public async Task CreateSuperPower_WithEmptyName_ShouldReturnBadRequest()
    {
        // Arrange
        var command = new CreateSuperPowerCommand(
            Name: string.Empty,
            Description: "Descrição"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/superpowers", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSuperPower_WithNullDescription_ShouldCreateSuperPower()
    {
        // Arrange
        var command = new CreateSuperPowerCommand(
            Name: "Invisibilidade",
            Description: null
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/superpowers", command);
        var result = await response.Content.ReadFromJsonAsync<ApiResult<SuperPowerDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result!.Data!.Description.Should().BeNull();
    }

    [Fact]
    public async Task UpdateSuperPower_WithValidCommand_ShouldUpdateSuperPower()
    {
        // Arrange
        var superPowerId = await SeedSingleSuperPowerAsync();

        var command = new UpdateSuperPowerCommand
        {
            Name = "Força aprimorada",
            Description = "Nível de força e poder sobre-humanos"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/superpowers/{superPowerId}", command);
        var result = await response.Content.ReadFromJsonAsync<ApiResult<SuperPowerDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data!.Name.Should().Be(command.Name);
        result.Data.Description.Should().Be(command.Description);
    }

    [Fact]
    public async Task UpdateSuperPower_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var command = new UpdateSuperPowerCommand
        {
            Name = "Teste",
            Description = "Teste"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/superpowers/9999", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteSuperPower_WithValidId_ShouldDeleteSuperPower()
    {
        // Arrange
        var superPowerId = await SeedSingleSuperPowerAsync();

        // Act
        var response = await _client.DeleteAsync($"/api/superpowers/{superPowerId}");
        var result = await response.Content.ReadFromJsonAsync<ApiResult<SuperPowerDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();

        // Verify superpower is deleted
        var getResponse = await _client.GetAsync($"/api/superpowers/{superPowerId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteSuperPower_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/superpowers/9999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateAndGetSuperPower_EndToEndFlow_ShouldWork()
    {
        // Arrange
        var createCommand = new CreateSuperPowerCommand(
            Name: "Manipulação do Tempo",
            Description: "Controle sobre o fluxo de tempo"
        );

        // Act - Create
        var createResponse = await _client.PostAsJsonAsync("/api/superpowers", createCommand);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResult<SuperPowerDto>>();

        // Act - Get
        var getResponse = await _client.GetAsync($"/api/superpowers/{createResult!.Data!.Id}");
        var getResult = await getResponse.Content.ReadFromJsonAsync<ApiResult<SuperPowerDto>>();

        // Assert
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        getResult!.Data!.Name.Should().Be(createCommand.Name);
        getResult.Data.Description.Should().Be(createCommand.Description);
    }

    [Fact]
    public async Task CreateSuperPower_WithDuplicateName_ShouldReturnBadRequest()
    {
        // Arrange
        await SeedSingleSuperPowerAsync();

        var command = new CreateSuperPowerCommand(
            Name: "Superforça", // Duplicado
            Description: "Mas com outra descrição"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/superpowers", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateAndDelete_CompleteFlow_ShouldWork()
    {
        // Arrange - Create
        var superPowerId = await SeedSingleSuperPowerAsync();

        // Act - Update
        var updateCommand = new UpdateSuperPowerCommand
        {
            Name = "Força Ultra",
            Description = "Força de nível máximo"
        };
        var updateResponse = await _client.PutAsJsonAsync($"/api/superpowers/{superPowerId}", updateCommand);

        // Act - Verify Update
        var getResponse = await _client.GetAsync($"/api/superpowers/{superPowerId}");
        var getResult = await getResponse.Content.ReadFromJsonAsync<ApiResult<SuperPowerDto>>();

        // Act - Delete
        var deleteResponse = await _client.DeleteAsync($"/api/superpowers/{superPowerId}");

        // Act - Verify Delete
        var finalGetResponse = await _client.GetAsync($"/api/superpowers/{superPowerId}");

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getResult!.Data!.Name.Should().Be(updateCommand.Name);
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        finalGetResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllSuperPowers_ShouldReturnPaginatedResults()
    {
        // Arrange
        await SeedManySuperPowersAsync(15);

        // Act
        var response = await _client.GetAsync("/api/superpowers");
        var result = await response.Content.ReadFromJsonAsync<ApiResult<IEnumerable<SuperPowerDto>>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result!.Data.Should().HaveCount(15);
    }

    // Helper methods
    private async Task<int> SeedSingleSuperPowerAsync()
    {
        return await _factory.ExecuteDbContextAsync(async context =>
        {
            var superPower = SuperPower.Create("Superforça", "Força física incrível");
            context.SuperPowers.Add(superPower);
            await context.SaveChangesAsync();
            return superPower.Id;
        });
    }

    private async Task SeedSuperPowersAsync()
    {
        await _factory.ExecuteDbContextAsync(async context =>
        {
            var powers = new[]
            {
                SuperPower.Create("Voo", "Capacidade de voar"),
                SuperPower.Create("Super velocidade", "Mover-se a velocidades incríveis"),
                SuperPower.Create("Visão de raio-x", "Ver através de objetos sólidos")
            };

            context.SuperPowers.AddRange(powers);
            await context.SaveChangesAsync();
        });
    }

    private async Task SeedManySuperPowersAsync(int count)
    {
        await _factory.ExecuteDbContextAsync(async context =>
        {
            var powers = Enumerable.Range(1, count)
                .Select(i => SuperPower.Create($"Poder {i}", $"Descrição {i}"))
                .ToArray();

            context.SuperPowers.AddRange(powers);
            await context.SaveChangesAsync();
        });
    }
}