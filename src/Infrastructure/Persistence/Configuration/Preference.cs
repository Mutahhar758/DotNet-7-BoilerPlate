using Demo.WebApi.Domain.Preference;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Demo.WebApi.Infrastructure.Persistence.Configuration;
public class CityConfig : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder
            .ToTable("City", SchemaNames.Preference);

        builder
            .Property(c => c.Name)
                .HasMaxLength(256);
    }
}

public class StateConfig : IEntityTypeConfiguration<State>
{
    public void Configure(EntityTypeBuilder<State> builder)
    {
        builder
            .ToTable("State", SchemaNames.Preference);

        builder
            .Property(s => s.Name)
                .HasMaxLength(256);
    }
}

public class CountryConfig : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder
            .ToTable("Country", SchemaNames.Preference);

        builder
            .Property(c => c.Name)
                .HasMaxLength(256);
    }
}