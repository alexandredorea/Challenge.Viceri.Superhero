using Challenge.Viceri.Superhero.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Challenge.Viceri.Superhero.Infrastructure.Persistences.Configurations;

internal sealed class HeroConfiguration : IEntityTypeConfiguration<Hero>
{
    public void Configure(EntityTypeBuilder<Hero> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(h => h.Name)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(h => h.Codename)
            .HasMaxLength(120)
            .IsRequired();
    }
}