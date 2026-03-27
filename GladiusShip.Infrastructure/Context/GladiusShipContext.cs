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


    public virtual DbSet<Expertise> Expertise { get; set; }

    public virtual DbSet<Insurance> Insurance { get; set; }

    public virtual DbSet<InsuranceDetails> InsuranceDetails { get; set; }

    public virtual DbSet<InsuranceDiscount> InsuranceDiscount { get; set; }

    public virtual DbSet<Log> Log { get; set; }

    public virtual DbSet<Maintenance> Maintenance { get; set; }

    public virtual DbSet<MaintenanceCostDocument> MaintenanceCostDocument { get; set; }

    public virtual DbSet<MaintenanceDetails> MaintenanceDetails { get; set; }

    public virtual DbSet<Marina> Marina { get; set; }

    public virtual DbSet<MarinaDetails> MarinaDetails { get; set; }

    public virtual DbSet<MaintenanceJob> MaintenanceJob { get; set; }

    public virtual DbSet<MaintenanceJobItem> MaintenanceJobItem { get; set; }

    public virtual DbSet<MaintenanceJobRisk> MaintenanceJobRisk { get; set; }

    public virtual DbSet<MaintenanceJobCost> MaintenanceJobCost { get; set; }

    public virtual DbSet<MaintenanceJobPhoto> MaintenanceJobPhoto { get; set; }

    public virtual DbSet<MaintenanceJobAction> MaintenanceJobAction { get; set; }

    public virtual DbSet<MarinaPermission> MarinaPermission { get; set; }

    public virtual DbSet<MarinaPrice> MarinaPrice { get; set; }

    public virtual DbSet<MarinaRoad> MarinaRoad { get; set; }

    public virtual DbSet<Port> Port { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleDetail> RoleDetails { get; set; }

    public virtual DbSet<Ship> Ship { get; set; }

    public virtual DbSet<ShipDocument> ShipDocument { get; set; }

    public virtual DbSet<ShipMachine> ShipMachine { get; set; }

    public virtual DbSet<ShipPermission> ShipPermission { get; set; }

    public virtual DbSet<ShipPhoto> ShipPhoto { get; set; }

    public virtual DbSet<ShipRegistration> ShipRegistration { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");
        });

        modelBuilder.Entity<RoleDetail>(entity =>
        {
            entity.ToTable("RoleDetails");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
