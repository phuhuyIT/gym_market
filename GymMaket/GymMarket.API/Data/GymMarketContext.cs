using GymMarket.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GymMarket.API.Data;

public partial class GymMarketContext : IdentityDbContext<AppUser>
{
    public GymMarketContext(DbContextOptions<GymMarketContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
        base.OnModelCreating(modelBuilder);

        foreach(var entityType in modelBuilder.Model.GetEntityTypes())
        {
            string? tableName = entityType.GetTableName();
            if(tableName != null && tableName.StartsWith("AspNet"))
            {
                entityType.SetTableName(tableName.Substring(6));
            }
        }

        modelBuilder.SeedData();

    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
