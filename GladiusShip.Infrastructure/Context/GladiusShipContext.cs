using System;
using System.Collections.Generic;
using GladiusShip.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;

namespace GladiusShip.Infrastructure.Context;

public partial class GladiusShipContext : DbContext
{
    public GladiusShipContext()
    {
    }

    public GladiusShipContext(DbContextOptions<GladiusShipContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Log> Log { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleDetail> RoleDetails { get; set; }

    public virtual DbSet<Ship> Ship { get; set; }

    public virtual DbSet<ShipDocument> ShipDocument { get; set; }

    public virtual DbSet<ShipMachine> ShipMachine { get; set; }

    public virtual DbSet<ShipPhoto> ShipPhoto { get; set; }

    public virtual DbSet<ShipRegistration> ShipRegistration { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Role");
        });

        modelBuilder.Entity<RoleDetail>(entity =>
        {
            entity.HasNoKey();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasNoKey();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
