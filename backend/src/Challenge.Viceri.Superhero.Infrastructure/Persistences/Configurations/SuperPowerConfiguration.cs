using Challenge.Viceri.Superhero.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Challenge.Viceri.Superhero.Infrastructure.Persistences.Configurations;

internal sealed class SuperPowerConfiguration : IEntityTypeConfiguration<SuperPower>
{
    public void Configure(EntityTypeBuilder<SuperPower> builder)
    {
        builder.HasKey(sp => sp.Id);
        builder.Property(sp => sp.Id).ValueGeneratedOnAdd();

        builder.Property(sp => sp.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(sp => sp.Description)
            .HasMaxLength(250)
            .IsRequired(false); // Descricao é opcional (Allow Nulls)
    }
}