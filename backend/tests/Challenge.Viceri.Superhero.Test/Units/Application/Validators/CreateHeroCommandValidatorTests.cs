using System.Linq.Expressions;
using Challenge.Viceri.Superhero.Application.Interfaces;
using Challenge.Viceri.Superhero.Application.UseCases.Heros.Commands;
using Challenge.Viceri.Superhero.Application.UseCases.Heros.Validations;
using Challenge.Viceri.Superhero.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;

namespace Challenge.Viceri.Superhero.Test.Units.Application.Validators;

public class CreateHeroCommandValidatorTests
{
    private readonly Mock<ISuperheroContext> _contextMock;
    private readonly Mock<DbSet<Hero>> _heroesDbSetMock;
    private readonly Mock<DbSet<SuperPower>> _superPowersDbSetMock;
    private readonly CreateHeroCommandValidator _validator;

    public CreateHeroCommandValidatorTests()
    {
        _contextMock = new Mock<ISuperheroContext>();
        _heroesDbSetMock = new Mock<DbSet<Hero>>();
        _superPowersDbSetMock = new Mock<DbSet<SuperPower>>();

        _contextMock.Setup(c => c.Heroes).Returns(_heroesDbSetMock.Object);
        _contextMock.Setup(c => c.SuperPowers).Returns(_superPowersDbSetMock.Object);

        _validator = new CreateHeroCommandValidator(_contextMock.Object);
    }

    [Fact]
    public async Task Validate_WithValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new CreateHeroCommand(
            Name: "Peter Parker",
            Codename: "Homem-Aranha",
            SuperpowerIds: new List<int> { 1, 2 },
            DateBirth: new DateTime(1995, 8, 10),
            Height: 1.78f,
            Weight: 76.5f
        );

        SetupHeroesDbSet(new List<Hero>());
        SetupSuperPowersDbSet(new List<SuperPower>
        {
            SuperPower.Create("Superforça", ""),
            SuperPower.Create("Lançamento de Teias", "")
        });

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_WithEmptyName_ShouldHaveError()
    {
        // Arrange
        var command = new CreateHeroCommand(
            Name: "",
            Codename: "Homem-Aranha",
            SuperpowerIds: new List<int> { 1 },
            DateBirth: null,
            Height: 1.78f,
            Weight: 76.5f
        );

        SetupHeroesDbSet(new List<Hero>());
        SetupSuperPowersDbSet(new List<SuperPower> { SuperPower.Create("Power", "") });

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validate_WithNameTooLong_ShouldHaveError()
    {
        // Arrange
        var longName = new string('A', 121);
        var command = new CreateHeroCommand(
            Name: longName,
            Codename: "Hero",
            SuperpowerIds: new List<int> { 1 },
            DateBirth: null,
            Height: 1.78f,
            Weight: 76.5f
        );

        SetupHeroesDbSet(new List<Hero>());
        SetupSuperPowersDbSet(new List<SuperPower> { SuperPower.Create("Power", "") });

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validate_WithEmptyCodename_ShouldHaveError()
    {
        // Arrange
        var command = new CreateHeroCommand(
            Name: "Peter Parker",
            Codename: "",
            SuperpowerIds: new List<int> { 1 },
            DateBirth: null,
            Height: 1.78f,
            Weight: 76.5f
        );

        SetupHeroesDbSet(new List<Hero>());
        SetupSuperPowersDbSet(new List<SuperPower> { SuperPower.Create("Power", "") });

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithDuplicateCodename_ShouldHaveError()
    {
        // Arrange
        var existingHero = Hero.Create("Bruce Wayne", "Batman", 1.88f, 95.0f);
        var command = new CreateHeroCommand(
            Name: "Dick Grayson",
            Codename: "Batman",
            SuperpowerIds: new List<int> { 1 },
            DateBirth: null,
            Height: 1.75f,
            Weight: 79.0f
        );

        SetupHeroesDbSet(new List<Hero> { existingHero });
        SetupSuperPowersDbSet(new List<SuperPower> { SuperPower.Create("Power", "") });

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithEmptySuperPowerIds_ShouldHaveError()
    {
        // Arrange
        var command = new CreateHeroCommand(
            Name: "Peter Parker",
            Codename: "Homem-Aranha",
            SuperpowerIds: new List<int>(),
            DateBirth: null,
            Height: 1.78f,
            Weight: 76.5f
        );

        SetupHeroesDbSet(new List<Hero>());
        SetupSuperPowersDbSet(new List<SuperPower>());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithDuplicateSuperPowerIds_ShouldHaveError()
    {
        // Arrange
        var command = new CreateHeroCommand(
            Name: "Peter Parker",
            Codename: "Homem-Aranha",
            SuperpowerIds: new List<int> { 1, 1, 2, 2 },
            DateBirth: null,
            Height: 1.78f,
            Weight: 76.5f
        );

        SetupHeroesDbSet(new List<Hero>());
        SetupSuperPowersDbSet(new List<SuperPower>());

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithInvalidHeight_ShouldHaveError()
    {
        // Arrange
        var command = new CreateHeroCommand(
            Name: "Peter Parker",
            Codename: "Homem-Aranha",
            SuperpowerIds: new List<int> { 1 },
            DateBirth: null,
            Height: 0,
            Weight: 76.5f
        );

        SetupHeroesDbSet(new List<Hero>());
        SetupSuperPowersDbSet(new List<SuperPower> { SuperPower.Create("Power", "") });

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Height");
    }

    [Fact]
    public async Task Validate_WithInvalidWeight_ShouldHaveError()
    {
        // Arrange
        var command = new CreateHeroCommand(
            Name: "Peter Parker",
            Codename: "Homem-Aranha",
            SuperpowerIds: new List<int> { 1 },
            DateBirth: null,
            Height: 1.78f,
            Weight: 0
        );

        SetupHeroesDbSet(new List<Hero>());
        SetupSuperPowersDbSet(new List<SuperPower> { SuperPower.Create("Power", "") });

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Weight");
    }

    [Fact]
    public async Task Validate_WithFutureDateBirth_ShouldHaveError()
    {
        // Arrange
        var command = new CreateHeroCommand(
            Name: "Peter Parker",
            Codename: "Homem-Aranha",
            SuperpowerIds: new List<int> { 1 },
            DateBirth: DateTime.UtcNow.AddDays(1),
            Height: 1.78f,
            Weight: 76.5f
        );

        SetupHeroesDbSet(new List<Hero>());
        SetupSuperPowersDbSet(new List<SuperPower> { SuperPower.Create("Power", "") });

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithTooYoungHero_ShouldHaveError()
    {
        // Arrange
        var command = new CreateHeroCommand(
            Name: "Peter Parker",
            Codename: "Homem-Aranha",
            SuperpowerIds: new List<int> { 1 },
            DateBirth: DateTime.UtcNow.AddYears(-10),
            Height: 1.78f,
            Weight: 76.5f
        );

        SetupHeroesDbSet(new List<Hero>());
        SetupSuperPowersDbSet(new List<SuperPower> { SuperPower.Create("Power", "") });

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
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

    private void SetupSuperPowersDbSet(List<SuperPower> superPowers)
    {
        var queryable = superPowers.AsQueryable();
        _superPowersDbSetMock.As<IQueryable<SuperPower>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<SuperPower>(queryable.Provider));
        _superPowersDbSetMock.As<IQueryable<SuperPower>>().Setup(m => m.Expression).Returns(queryable.Expression);
        _superPowersDbSetMock.As<IQueryable<SuperPower>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        _superPowersDbSetMock.As<IQueryable<SuperPower>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
        _superPowersDbSetMock.As<IAsyncEnumerable<SuperPower>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<SuperPower>(queryable.GetEnumerator()));
    }
}

// Helper
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
    public TestAsyncEnumerable(Expression expression) : base(expression)
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