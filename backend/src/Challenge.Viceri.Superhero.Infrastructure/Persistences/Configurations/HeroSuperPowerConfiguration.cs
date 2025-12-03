using Challenge.Viceri.Superhero.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Challenge.Viceri.Superhero.Infrastructure.Persistences.Configurations;

internal sealed class HeroSuperPowerConfiguration : IEntityTypeConfiguration<HeroSuperPower>
{
    public void Configure(EntityTypeBuilder<HeroSuperPower> builder)
    {
        builder.HasKey(hsp => new { hsp.HeroId, hsp.SuperPowerId });

        builder.HasOne(hsp => hsp.Hero)
            .WithMany(h => h.HeroSuperPowers)
            .HasForeignKey(hsp => hsp.HeroId);

        builder.HasOne(hsp => hsp.SuperPower)
            .WithMany(sp => sp.HeroSuperPowers)
            .HasForeignKey(hsp => hsp.SuperPowerId);
    }
}