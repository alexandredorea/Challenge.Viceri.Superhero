using Challenge.Viceri.Superhero.Domain.Entities;
using FluentAssertions;

namespace Challenge.Viceri.Superhero.Test.Units.Domain;

public sealed class HeroTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateHero()
    {
        // Arrange
        var name = "Peter Parker";
        var codename = "Homem-Aranha";
        var height = 1.78f;
        var weight = 76.5f;
        var dateBirth = new DateTime(1995, 8, 10);

        // Act
        var hero = Hero.Create(name, codename, height, weight, dateBirth);

        // Assert
        hero.Should().NotBeNull();
        hero.Name.Should().Be(name);
        hero.Codename.Should().Be(codename);
        hero.Height.Should().Be(height);
        hero.Weight.Should().Be(weight);
        hero.DateBirth.Should().Be(dateBirth);
        hero.HeroSuperPowers.Should().BeEmpty();
    }

    [Fact]
    public void Create_WithWhitespace_ShouldTrimValues()
    {
        // Arrange
        var name = "  Bruce Wayne  ";
        var codename = "  Batman  ";
        var height = 1.88f;
        var weight = 95.0f;

        // Act
        var hero = Hero.Create(name, codename, height, weight);

        // Assert
        hero.Name.Should().Be("Bruce Wayne");
        hero.Codename.Should().Be("Batman");
    }

    [Fact]
    public void Create_WithoutDateBirth_ShouldCreateHeroWithNullDateBirth()
    {
        // Arrange
        var name = "Anthony Edward Stark";
        var codename = "Homem de Ferro";
        var height = 1.85f;
        var weight = 90.0f;

        // Act
        var hero = Hero.Create(name, codename, height, weight);

        // Assert
        hero.DateBirth.Should().BeNull();
    }

    [Fact]
    public void AddSuperPowers_WithValidIds_ShouldAddPowers()
    {
        // Arrange
        var hero = Hero.Create("Clark Kent", "Superman", 1.90f, 107.0f);
        var superPowerIds = new List<int> { 1, 2, 3 };

        // Act
        hero.AddSuperPowers(superPowerIds);

        // Assert
        hero.HeroSuperPowers.Should().HaveCount(3);
        hero.HeroSuperPowers.Select(hsp => hsp.SuperPowerId).Should().BeEquivalentTo(superPowerIds);
    }

    [Fact]
    public void AddSuperPowers_WithEmptyList_ShouldNotAddAnyPowers()
    {
        // Arrange
        var hero = Hero.Create("Diana Prince", "Mulher Maravilha", 1.83f, 75.0f);
        var superPowerIds = new List<int>();

        // Act
        hero.AddSuperPowers(superPowerIds);

        // Assert
        hero.HeroSuperPowers.Should().BeEmpty();
    }

    [Fact]
    public void UpdateSuperPowers_ShouldReplaceExistingPowers()
    {
        // Arrange
        var hero = Hero.Create("Barry Allen", "Flash", 1.83f, 81.0f);
        hero.AddSuperPowers(new List<int> { 1, 2, 3 });
        var newSuperPowerIds = new List<int> { 4, 5 };

        // Act
        hero.UpdateSuperPowers(newSuperPowerIds);

        // Assert
        hero.HeroSuperPowers.Should().HaveCount(2);
        hero.HeroSuperPowers.Select(hsp => hsp.SuperPowerId).Should().BeEquivalentTo(newSuperPowerIds);
    }

    [Fact]
    public void UpdateSuperPowers_WithDuplicates_ShouldKeepOnlyDistinctIds()
    {
        // Arrange
        var hero = Hero.Create("Hal Jordan", "Lanterna Verde", 1.88f, 90.0f);
        var superPowerIds = new List<int> { 1, 2, 2, 3, 3, 3 };

        // Act
        hero.UpdateSuperPowers(superPowerIds);

        // Assert
        hero.HeroSuperPowers.Should().HaveCount(3);
        hero.HeroSuperPowers.Select(hsp => hsp.SuperPowerId).Should().BeEquivalentTo(new[] { 1, 2, 3 });
    }

    [Fact]
    public void UpdateSuperPowers_WithPartialOverlap_ShouldMergeCorrectly()
    {
        // Arrange
        var hero = Hero.Create("Arthur Curry", "Aquaman", 1.88f, 145.0f);
        hero.AddSuperPowers(new List<int> { 1, 2, 3 });
        var newSuperPowerIds = new List<int> { 2, 3, 4, 5 };

        // Act
        hero.UpdateSuperPowers(newSuperPowerIds);

        // Assert
        hero.HeroSuperPowers.Should().HaveCount(4);
        hero.HeroSuperPowers.Select(hsp => hsp.SuperPowerId).Should().BeEquivalentTo(new[] { 2, 3, 4, 5 });
    }

    [Fact]
    public void UpdateHero_WithValidParameters_ShouldUpdateAllProperties()
    {
        // Arrange
        var hero = Hero.Create("Bruce Banner", "Hulk", 1.75f, 58.0f);
        var newName = "Robert Bruce Banner";
        var newCodename = "O Incrivel Hulk";
        var newHeight = 2.44f;
        var newWeight = 635.0f;
        var newDateBirth = new DateTime(1969, 12, 18);

        // Act
        hero.UpdateHero(newName, newCodename, newHeight, newWeight, newDateBirth);

        // Assert
        hero.Name.Should().Be(newName);
        hero.Codename.Should().Be(newCodename);
        hero.Height.Should().Be(newHeight);
        hero.Weight.Should().Be(newWeight);
        hero.DateBirth.Should().Be(newDateBirth);
    }

    [Fact]
    public void UpdateHero_WithWhitespace_ShouldTrimValues()
    {
        // Arrange
        var hero = Hero.Create("Scott Summers", "Ciclope", 1.88f, 88.0f);

        // Act
        hero.UpdateHero("  Scott Summers  ", "  Ciclope  ", 1.88f, 88.0f);

        // Assert
        hero.Name.Should().Be("Scott Summers");
        hero.Codename.Should().Be("Ciclope");
    }

    [Fact]
    public void UpdateHero_WithNullDateBirth_ShouldSetDateBirthToNull()
    {
        // Arrange
        var hero = Hero.Create("Jean Grey", "Phoenix", 1.68f, 52.0f, new DateTime(1980, 5, 15));

        // Act
        hero.UpdateHero("Jean Grey", "Phoenix", 1.68f, 52.0f, null);

        // Assert
        hero.DateBirth.Should().BeNull();
    }

    [Fact]
    public void HeroSuperPowers_ShouldBeReadOnly()
    {
        // Arrange
        var hero = Hero.Create("Steve Rogers", "Captão America", 1.88f, 109.0f);
        hero.AddSuperPowers(new List<int> { 1, 2 });

        // Act
        var powers = hero.HeroSuperPowers;

        // Assert
        powers.Should().BeAssignableTo<IReadOnlyCollection<HeroSuperPower>>();
    }
}