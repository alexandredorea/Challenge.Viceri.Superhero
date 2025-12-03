namespace Challenge.Viceri.Superhero.Domain.Entities;

public sealed class HeroSuperPower
{
    public int HeroId { get; private set; }
    public int SuperPowerId { get; private set; }
    public Hero Hero { get; init; } = default!;
    public SuperPower SuperPower { get; init; } = default!;

    private HeroSuperPower()
    {
    }

    private HeroSuperPower(int heroId, int superPowerId)
    {
        HeroId = heroId;
        SuperPowerId = superPowerId;
    }

    public static HeroSuperPower Create(int heroId, int superPowerId)
        => new(heroId, superPowerId);
}