using AxoMotor.ApiServer.Models.Catalog;
using Microsoft.EntityFrameworkCore;

namespace AxoMotor.ApiServer.Data;

public class AxoMotorContext : DbContext
{
    public AxoMotorContext(DbContextOptions options) : base(options)
    { }

    public DbSet<DeviceEventInfo> DeviceEventCatalog { get; set; }

    public DbSet<IncidentInfo> IncidentCatalog { get; set; }

    public DbSet<KnownLocation> KnownLocations { get; set; }

    public DbSet<KPI> KPICatalog { get; set; }

    public DbSet<VehicleClass> VehicleClasses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DeviceEventInfo>(entity =>
        {
            entity.ToTable("device_event_catalog").HasKey(x => x.Code);
            entity.Property(x => x.Code).HasMaxLength(24);
            entity.Property(x => x.DisplayName).HasMaxLength(32);
            entity.Property(x => x.Description).HasMaxLength(64);
            entity.Property(x => x.Type).HasConversion<string>();
            entity.Property(x => x.Severity).HasConversion<string>();
        });
        
        modelBuilder.Entity<IncidentInfo>(entity =>
        {
            entity.ToTable("incident_catalog").HasKey(x => x.Code);
            entity.Property(x => x.Code).HasMaxLength(24);
            entity.Property(x => x.DisplayName).HasMaxLength(32);
            entity.Property(x => x.Description).HasMaxLength(64);
            entity.Property(x => x.Priority).HasConversion<string>();
            entity.Property(x => x.Type).HasConversion<string>();
        });
        
        modelBuilder.Entity<KnownLocation>(entity =>
        {
            entity.ToTable("known_locations").HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(64);
            entity.Property(x => x.Address).HasMaxLength(256);
            entity.Property(x => x.Longitude).HasPrecision(9, 6);
            entity.Property(x => x.Latitude).HasPrecision(9, 6);
            entity.Property(x => x.Ratio);
        });
        
        modelBuilder.Entity<KPI>(entity =>
        {
            entity.ToTable("kpi_catalog").HasKey(x => x.Code);
            entity.Property(x => x.Type).HasConversion<string>();
            entity.Property(x => x.DisplayName).HasMaxLength(32);
        });
        
        modelBuilder.Entity<VehicleClass>(entity => 
        {
            entity.ToTable("vehicle_classes").HasKey(x => x.Code);
            entity.Property(x => x.DisplayName).HasMaxLength(24);
        });
    }
}
