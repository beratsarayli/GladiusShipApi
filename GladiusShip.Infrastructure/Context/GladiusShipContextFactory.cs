using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GladiusShip.Infrastructure.Context;

public class GladiusShipContextFactory : IDesignTimeDbContextFactory<GladiusShipContext>
{
    public GladiusShipContext CreateDbContext(string[] args)
    {
        //var optionsBuilder = new DbContextOptionsBuilder<GladiusShipContext>();
        //optionsBuilder.UseSqlServer("Server=BERAT;Database=GladiusShip;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true");

        var optionsBuilder = new DbContextOptionsBuilder<GladiusShipContext>();
        optionsBuilder.UseSqlServer("Server=NSWLBMGGNATRO\\MSSQLSERVER2019;Database=GladiusShip;User Id=gladius_app;Password=15*14Ber!;MultipleActiveResultSets=true;TrustServerCertificate=true;");

        return new GladiusShipContext(optionsBuilder.Options);
    }
}