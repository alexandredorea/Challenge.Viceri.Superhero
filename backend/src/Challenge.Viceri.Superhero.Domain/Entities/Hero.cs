namespace Challenge.Viceri.Superhero.Domain.Entities;

public sealed class Hero
{
    private readonly List<HeroSuperPower> _heroSuperPowers = []; //Backing field para encapsulamento adequado
    public int Id { get; init; }
    public string Name { get; private set; } = default!;
    public string Codename { get; private set; } = default!;
    public DateTime? DateBirth { get; private set; }
    public float Height { get; private set; }
    public float Weight { get; private set; }
    public IReadOnlyCollection<HeroSuperPower> HeroSuperPowers => _heroSuperPowers.AsReadOnly();

    private Hero()
    { }

    private Hero(
        string name,
        string codename,
        float height,
        float weight,
        DateTime? dateBirth = null)
    {
        Name = name.Trim();
        Codename = codename.Trim();
        Height = height;
        Weight = weight;
        DateBirth = dateBirth;
    }

    public static Hero Create(
        string name,
        string codename,
        float height,
        float weight,
        DateTime? dateBirth = null)
    {
        return new(
            name,
            codename,
            height,
            weight,
            dateBirth);
    }

    public void AddSuperPowers(List<int> superPowerIds)
    {
        foreach (var superPowerId in superPowerIds)
            _heroSuperPowers.Add(HeroSuperPower.Create(Id, superPowerId));
    }

    public void UpdateSuperPowers(List<int> superPowerIds)
    {
        var distinctIds = superPowerIds.Distinct().ToList();
        var existingIds = _heroSuperPowers.Select(hsp => hsp.SuperPowerId).ToHashSet();
        var newIds = distinctIds.ToHashSet();

        _heroSuperPowers.RemoveAll(hsp => !newIds.Contains(hsp.SuperPowerId));

        foreach (var superPowerId in newIds.Where(id => !existingIds.Contains(id)))
            _heroSuperPowers.Add(HeroSuperPower.Create(Id, superPowerId));

        //_heroSuperPowers.Clear();
        //foreach (var superPowerId in superPowerIds)
        //    _heroSuperPowers.Add(HeroSuperPower.Create(Id, superPowerId));
    }

    public void UpdateHero(
        string name,
        string codename,
        float height,
        float weight,
        DateTime? dateBirth = null)
    {
        Name = name.Trim();
        Codename = codename.Trim();
        Height = height;
        Weight = weight;
        DateBirth = dateBirth;
    }
}