namespace Challenge.Viceri.Superhero.Domain.Entities;

public sealed record SuperPower
{
    private readonly List<HeroSuperPower> _heroSuperPowers = []; //Backing field para encapsulamento adequado
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public string? Description { get; init; }
    public IReadOnlyCollection<HeroSuperPower> HeroSuperPowers => _heroSuperPowers.AsReadOnly();

    private SuperPower()
    {
    }

    private SuperPower(string name, string? description)
    {
        Name = name;
        Description = description;
    }

    public static SuperPower Create(string name, string? description)
    {
        return new SuperPower(name, description);
    }
}