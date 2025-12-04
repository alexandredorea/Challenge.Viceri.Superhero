using Challenge.Viceri.Superhero.Application.Interfaces;
using Challenge.Viceri.Superhero.Application.UseCases.Heros.Commands;
using Challenge.Viceri.Superhero.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;

namespace Challenge.Viceri.Superhero.Test.Units.Applications.Handlers;

public class UpdateHeroCommandHandlerTests
{
    private readonly Mock<ISuperheroContext> _contextMock;
    private readonly Mock<DbSet<Hero>> _heroesDbSetMock;
    private readonly UpdateHeroCommandHandler _handler;

    public UpdateHeroCommandHandlerTests()
    {
        _contextMock = new Mock<ISuperheroContext>();
        _heroesDbSetMock = new Mock<DbSet<Hero>>();

        _contextMock.Setup(c => c.Heroes).Returns(_heroesDbSetMock.Object);
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _handler = new UpdateHeroCommandHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_WithExistingHero_ShouldUpdateHero()
    {
        // Arrange
        var existingHero = Hero.Create("Peter Parker", "Spider-Man", 1.78f, 76.5f);
        existingHero.AddSuperPowers(new List<int> { 1, 2 });

        var command = new UpdateHeroCommand
        {
            Name = "Peter Benjamin Parker",
            Codename = "The Amazing Spider-Man",
            SuperpowerIds = new List<int> { 1, 2, 3 },
            DateBirth = new DateTime(1995, 8, 10),
            Height = 1.80f,
            Weight = 78.0f
        };
        command.SetHeroId(1);

        SetupHeroesDbSet(new List<Hero> { existingHero });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        existingHero.Name.Should().Be(command.Name);
        existingHero.Codename.Should().Be(command.Codename);
        existingHero.Height.Should().Be(command.Height);
        existingHero.Weight.Should().Be(command.Weight);
        existingHero.DateBirth.Should().Be(command.DateBirth);
        existingHero.HeroSuperPowers.Should().HaveCount(3);

        _contextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistingHero_ShouldReturnNull()
    {
        // Arrange
        var command = new UpdateHeroCommand
        {
            Name = "Peter Parker",
            Codename = "Spider-Man",
            SuperpowerIds = new List<int> { 1 },
            DateBirth = null,
            Height = 1.78f,
            Weight = 76.5f
        };
        command.SetHeroId(999);

        SetupHeroesDbSet(new List<Hero>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _contextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldUpdateSuperPowers()
    {
        // Arrange
        var existingHero = Hero.Create("Bruce Wayne", "Batman", 1.88f, 95.0f);
        existingHero.AddSuperPowers(new List<int> { 1, 2, 3 });

        var command = new UpdateHeroCommand
        {
            Name = "Bruce Wayne",
            Codename = "Batman",
            SuperpowerIds = new List<int> { 3, 4, 5 }, // Partial overlap
            DateBirth = null,
            Height = 1.88f,
            Weight = 95.0f
        };
        command.SetHeroId(1);

        SetupHeroesDbSet(new List<Hero> { existingHero });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingHero.HeroSuperPowers.Should().HaveCount(3);
        existingHero.HeroSuperPowers.Select(hsp => hsp.SuperPowerId)
            .Should().BeEquivalentTo(new[] { 3, 4, 5 });
    }

    [Fact]
    public async Task Handle_WithWhitespace_ShouldTrimValues()
    {
        // Arrange
        var existingHero = Hero.Create("Tony Stark", "Homem de Ferro", 1.85f, 102.0f);

        var command = new UpdateHeroCommand
        {
            Name = "  Anthony Edward Stark  ",
            Codename = "  Homem de Ferro  ",
            SuperpowerIds = new List<int> { 1 },
            DateBirth = null,
            Height = 1.85f,
            Weight = 102.0f
        };
        command.SetHeroId(1);

        SetupHeroesDbSet(new List<Hero> { existingHero });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingHero.Name.Should().Be("Anthony Edward Stark");
        existingHero.Codename.Should().Be("Homem de Ferro");
    }

    [Fact]
    public async Task Handle_WithNullDateBirth_ShouldSetDateBirthToNull()
    {
        // Arrange
        var existingHero = Hero.Create("Clark Kent", "Superman", 1.90f, 107.0f, new DateTime(1980, 6, 18));

        var command = new UpdateHeroCommand
        {
            Name = "Clark Kent",
            Codename = "Superman",
            SuperpowerIds = new List<int> { 1 },
            DateBirth = null, // Removing date birth
            Height = 1.90f,
            Weight = 107.0f
        };
        command.SetHeroId(1);

        SetupHeroesDbSet(new List<Hero> { existingHero });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingHero.DateBirth.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnUpdatedHeroDto()
    {
        // Arrange
        var existingHero = Hero.Create("Diana Prince", "Mulher Maravilha", 1.83f, 75.0f);
        existingHero.AddSuperPowers(new List<int> { 1 });

        var command = new UpdateHeroCommand
        {
            Name = "Diana de Themyscira",
            Codename = "Mulher Maravilha",
            SuperpowerIds = new List<int> { 1, 2 },
            DateBirth = new DateTime(1918, 10, 21),
            Height = 1.83f,
            Weight = 75.0f
        };
        command.SetHeroId(1);

        SetupHeroesDbSet(new List<Hero> { existingHero });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Diana de Themyscira");
        result.Codename.Should().Be("Mulher Maravilha");
        result.SuperPowers.Should().HaveCount(2);
    }

    private void SetupHeroesDbSet(List<Hero> heroes)
    {
        var queryable = heroes.AsQueryable();
        _heroesDbSetMock.As<IQueryable<Hero>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Hero>(queryable.Provider));
        _heroesDbSetMock.As<IQueryable<Hero>>().Setup(m => m.Expression).Returns(queryable.Expression);
        _heroesDbSetMock.As<IQueryable<Hero>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        _heroesDbSetMock.As<IQueryable<Hero>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
        _heroesDbSetMock.As<IAsyncEnumerable<Hero>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<Hero>(queryable.GetEnumerator()));
    }
}

// Helper classes (mesmas do CreateHeroCommandValidatorTests.cs)
internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;

    internal TestAsyncQueryProvider(IQueryProvider inner)
    {
        _inner = inner;
    }

    public IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
    {
        return new TestAsyncEnumerable<TEntity>(expression);
    }

    public IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
    {
        return new TestAsyncEnumerable<TElement>(expression);
    }

    public object Execute(System.Linq.Expressions.Expression expression)
    {
        return _inner.Execute(expression);
    }

    public TResult Execute<TResult>(System.Linq.Expressions.Expression expression)
    {
        return _inner.Execute<TResult>(expression);
    }

    public TResult ExecuteAsync<TResult>(System.Linq.Expressions.Expression expression, CancellationToken cancellationToken = default)
    {
        var resultType = typeof(TResult).GetGenericArguments()[0];
        var executeMethod = typeof(IQueryProvider)
            .GetMethod(nameof(IQueryProvider.Execute), 1, new[] { typeof(System.Linq.Expressions.Expression) })
            ?.MakeGenericMethod(resultType);

        var result = executeMethod?.Invoke(_inner, new object[] { expression });
        return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))?.MakeGenericMethod(resultType).Invoke(null, new[] { result })!;
    }
}

internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(System.Linq.Expressions.Expression expression) : base(expression)
    {
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }

    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
}

internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public TestAsyncEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
    }

    public T Current => _inner.Current;

    public ValueTask<bool> MoveNextAsync()
    {
        return ValueTask.FromResult(_inner.MoveNext());
    }

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return ValueTask.CompletedTask;
    }
}