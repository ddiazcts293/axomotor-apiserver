using AxoMotor.ApiServer.Models.Catalog;
using Microsoft.EntityFrameworkCore;

namespace AxoMotor.ApiServer.Data;

public class AxoMotorContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<DeviceEventInfo> DeviceEventCatalog { get; set; }

    public DbSet<IncidentInfo> IncidentCatalog { get; set; }

    public DbSet<KnownLocation> KnownLocations { get; set; }

    public DbSet<KPI> KPICatalog { get; set; }

    public DbSet<VehicleClass> VehicleClasses { get; set; }

    public DbSet<Vehicle> Vehicles { get; set; }

    public DbSet<UserAccount> UserAccounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DeviceEventInfo>(entity =>
        {
            entity.ToTable("DeviceEventCatalog").HasKey(x => x.Code);
            entity.Property(x => x.Code).HasMaxLength(24);
            entity.Property(x => x.DisplayName).HasMaxLength(32);
            entity.Property(x => x.Description).HasMaxLength(64);
            entity.Property(x => x.Type).HasConversion<string>();
            entity.Property(x => x.Severity).HasConversion<string>();
        });

        modelBuilder.Entity<IncidentInfo>(entity =>
        {
            entity.ToTable("IncidentCatalog").HasKey(x => x.Code);
            entity.Property(x => x.Code).HasMaxLength(24);
            entity.Property(x => x.DisplayName).HasMaxLength(32);
            entity.Property(x => x.Description).HasMaxLength(64);
            entity.Property(x => x.Priority).HasConversion<string>();
            entity.Property(x => x.Type).HasConversion<string>();
        });

        modelBuilder.Entity<KnownLocation>(entity =>
        {
            entity.ToTable("KnownLocations").HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(64);
            entity.Property(x => x.Address).HasMaxLength(256);
            entity.Property(x => x.Longitude).HasPrecision(9, 6);
            entity.Property(x => x.Latitude).HasPrecision(9, 6);
            entity.Property(x => x.Ratio);
        });

        modelBuilder.Entity<KPI>(entity =>
        {
            entity.ToTable("KpiCatalog").HasKey(x => x.Code);
            entity.Property(x => x.Type).HasConversion<string>();
            entity.Property(x => x.DisplayName).HasMaxLength(32);
        });

        modelBuilder.Entity<VehicleClass>(entity =>
        {
            entity.ToTable("VehicleClasses").HasKey(x => x.Code);
            entity.Property(x => x.DisplayName).HasMaxLength(24);
            entity.HasMany<Vehicle>()
                .WithOne()
                .HasForeignKey(x => x.Class)
                .IsRequired();
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.ToTable("Vehicles").HasKey(x => x.Id);
            entity.Property(x => x.PlateNumber).HasMaxLength(10);
            entity.Property(x => x.RegistrationNumber).HasMaxLength(22);
            entity.Property(x => x.Status).HasConversion<string>();
            entity.Property(x => x.Brand).HasMaxLength(20);
            entity.Property(x => x.Model).HasMaxLength(20);
            entity.Property(x => x.Class).HasColumnName("ClassCode");
            entity.Property(x => x.RegistrationDate)
                .HasDefaultValueSql("curdate()");
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.Property(x => x.FirstName).HasMaxLength(40);
            entity.Property(x => x.LastName).HasMaxLength(40);
            entity.Property(x => x.Email).HasMaxLength(40);
            entity.Property(x => x.PhoneNumber).HasMaxLength(16);
            entity.Property(x => x.Role).HasConversion<string>();
            entity.Property(x => x.Status).HasConversion<string>();
            entity.Property(x => x.RegistrationDate)
                .HasDefaultValueSql("curdate()");
        });
    }
}
