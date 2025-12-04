using Challenge.Viceri.Superhero.Application.Interfaces;
using Challenge.Viceri.Superhero.Application.UseCases.Heros.Commands;
using Challenge.Viceri.Superhero.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Challenge.Viceri.Superhero.Test.Units.Application.Handlers;

public sealed class CreateHeroCommandHandlerTests
{
    private readonly Mock<ISuperheroContext> _contextMock;
    private readonly Mock<DbSet<Hero>> _heroesDbSetMock;
    private readonly CreateHeroCommandHandler _handler;

    public CreateHeroCommandHandlerTests()
    {
        _contextMock = new Mock<ISuperheroContext>();
        _heroesDbSetMock = new Mock<DbSet<Hero>>();

        _contextMock.Setup(c => c.Heroes).Returns(_heroesDbSetMock.Object);
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _handler = new CreateHeroCommandHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateHero()
    {
        // Arrange
        var command = new CreateHeroCommand(
            Name: "Peter Parker",
            Codename: "Homem-Aranha",
            SuperpowerIds: new List<int> { 1, 2, 3 },
            DateBirth: new DateTime(1995, 8, 10),
            Height: 1.78f,
            Weight: 76.5f
        );

        Hero? capturedHero = null;
        _heroesDbSetMock.Setup(m => m.Add(It.IsAny<Hero>()))
            .Callback<Hero>(h => capturedHero = h);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
        result.Codename.Should().Be(command.Codename);
        result.Height.Should().Be(command.Height);
        result.Weight.Should().Be(command.Weight);
        result.DateBirth.Should().Be(command.DateBirth);
        result.SuperPowers.Should().HaveCount(3);

        capturedHero.Should().NotBeNull();
        capturedHero!.HeroSuperPowers.Should().HaveCount(3);

        _heroesDbSetMock.Verify(m => m.Add(It.IsAny<Hero>()), Times.Once);
        _contextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNullDateBirth_ShouldCreateHeroWithNullDateBirth()
    {
        // Arrange
        var command = new CreateHeroCommand(
            Name: "Tony Stark",
            Codename: "Homem de Ferro",
            SuperpowerIds: new List<int> { 1 },
            DateBirth: null,
            Height: 1.85f,
            Weight: 90.0f
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.DateBirth.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldTrimNameAndCodename()
    {
        // Arrange
        var command = new CreateHeroCommand(
            Name: "  Bruce Wayne  ",
            Codename: "  Batman  ",
            SuperpowerIds: new List<int> { 1 },
            DateBirth: null,
            Height: 1.88f,
            Weight: 95.0f
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Name.Should().Be("Bruce Wayne");
        result.Codename.Should().Be("Batman");
    }

    [Fact]
    public async Task Handle_WithMultipleSuperPowers_ShouldAddAllPowers()
    {
        // Arrange
        var superPowerIds = new List<int> { 1, 2, 3, 4, 5 };
        var command = new CreateHeroCommand(
            Name: "Clark Kent",
            Codename: "Superman",
            SuperpowerIds: superPowerIds,
            DateBirth: new DateTime(1980, 6, 18),
            Height: 1.90f,
            Weight: 107.0f
        );

        Hero? capturedHero = null;
        _heroesDbSetMock.Setup(m => m.Add(It.IsAny<Hero>()))
            .Callback<Hero>(h => capturedHero = h);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedHero!.HeroSuperPowers.Should().HaveCount(5);
        capturedHero.HeroSuperPowers.Select(hsp => hsp.SuperPowerId)
            .Should().BeEquivalentTo(superPowerIds);
    }

    [Fact]
    public async Task Handle_ShouldCallSaveChangesAsync()
    {
        // Arrange
        var command = new CreateHeroCommand(
            Name: "Diana Prince",
            Codename: "Mulher Maravilha",
            SuperpowerIds: new List<int> { 1, 2 },
            DateBirth: null,
            Height: 1.83f,
            Weight: 75.0f
        );

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _contextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithCancellationToken_ShouldPassTokenToSaveChanges()
    {
        // Arrange
        var command = new CreateHeroCommand(
            Name: "Barry Allen",
            Codename: "The Flash",
            SuperpowerIds: new List<int> { 1 },
            DateBirth: null,
            Height: 1.83f,
            Weight: 81.0f
        );

        var cancellationToken = new CancellationToken();

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _contextMock.Verify(m => m.SaveChangesAsync(cancellationToken), Times.Once);
    }
}