using Challenge.Viceri.Superhero.Domain.Entities;
using FluentAssertions;

namespace Challenge.Viceri.Superhero.Test.Units.Domain;

public class SuperPowerTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateSuperPower()
    {
        // Arrange
        var name = "Superforça";
        var description = "Ability to lift enormous weights";

        // Act
        var superPower = SuperPower.Create(name, description);

        // Assert
        superPower.Should().NotBeNull();
        superPower.Name.Should().Be(name);
        superPower.Description.Should().Be(description);
        superPower.HeroSuperPowers.Should().BeEmpty();
    }

    [Fact]
    public void Create_WithNullDescription_ShouldCreateSuperPower()
    {
        // Arrange
        var name = "Voar";

        // Act
        var superPower = SuperPower.Create(name, null);

        // Assert
        superPower.Should().NotBeNull();
        superPower.Name.Should().Be(name);
        superPower.Description.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyDescription_ShouldCreateSuperPower()
    {
        // Arrange
        var name = "Telepatia";
        var description = string.Empty;

        // Act
        var superPower = SuperPower.Create(name, description);

        // Assert
        superPower.Name.Should().Be(name);
        superPower.Description.Should().BeEmpty();
    }

    [Fact]
    public void HeroSuperPowers_ShouldBeReadOnly()
    {
        // Arrange
        var superPower = SuperPower.Create("Invisibilidade", "Can become invisible");

        // Act
        var heroSuperPowers = superPower.HeroSuperPowers;

        // Assert
        heroSuperPowers.Should().BeAssignableTo<IReadOnlyCollection<HeroSuperPower>>();
    }

    [Theory]
    [InlineData("Super velocidade", "Mover-se a velocidades incríveis")]
    [InlineData("Visão de raio-x", "Ver através de objetos sólidos")]
    [InlineData("Healing Factor", "Rapid regeneration of damaged tissues")]
    public void Create_WithVariousInputs_ShouldCreateSuperPower(string name, string description)
    {
        // Act
        var superPower = SuperPower.Create(name, description);

        // Assert
        superPower.Name.Should().Be(name);
        superPower.Description.Should().Be(description);
    }

    [Fact]
    public void SuperPower_AsRecord_ShouldSupportEqualityComparison()
    {
        // Arrange
        var power1 = SuperPower.Create("Telekinesis", "Move objects with mind");
        var power2 = SuperPower.Create("Telekinesis", "Move objects with mind");

        // Act & Assert
        power1.Should().NotBeSameAs(power2);
    }
}