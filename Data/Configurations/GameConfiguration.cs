
using C___ASP.NET_Core_.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace C___ASP.NET_Core_.Data.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.Property(game => game.Price)
                .HasPrecision(5, 2); // (total digits (precision), digits to the right of the decimal (scale)) - goes up to 999.99
    }
}
