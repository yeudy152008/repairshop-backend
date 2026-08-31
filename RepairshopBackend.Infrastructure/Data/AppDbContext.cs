using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RepairshopBackend.Domain.Entities;

namespace RepairshopBackend.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<WorkOrderPart> WorkOrderParts => Set<WorkOrderPart>();
    public DbSet<InventoryCategory> InventoryCategories => Set<InventoryCategory>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<AccessLog> AccessLogs => Set<AccessLog>();
    public DbSet<MovementLog> MovementLogs => Set<MovementLog>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vehicle>()
            .HasOne(v => v.Customer)
            .WithMany(c => c.Vehicles)
            .HasForeignKey(v => v.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WorkOrder>()
            .HasOne(w => w.Vehicle)
            .WithMany()
            .HasForeignKey(w => w.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WorkOrder>()
            .HasMany(w => w.Parts)
            .WithOne(p => p.WorkOrder)
            .HasForeignKey(p => p.WorkOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WorkOrder>()
            .Property(w => w.LaborCost)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<WorkOrder>()
            .Property(w => w.HoursSpent)
            .HasColumnType("decimal(6,2)");

        modelBuilder.Entity<WorkOrderPart>()
            .Property(p => p.Cost)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<WorkOrderPart>()
            .HasOne(p => p.InventoryItem)
            .WithMany()
            .HasForeignKey(p => p.InventoryItemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InventoryItem>()
            .HasOne(i => i.Category)
            .WithMany()
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InventoryItem>()
            .Property(i => i.UnitCost)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<InventoryItem>()
            .Property(i => i.MarginPercent)
            .HasColumnType("decimal(5,2)");

        modelBuilder.Entity<InventoryItem>()
            .Property(i => i.SalePrice)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<InventoryItem>()
            .Property(i => i.IvaRate)
            .HasColumnType("decimal(5,2)");

        modelBuilder.Entity<Permission>()
            .HasIndex(p => p.Key)
            .IsUnique();

        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany()
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Purchase>()
            .HasOne(p => p.Supplier)
            .WithMany()
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Purchase>()
            .HasMany(p => p.Items)
            .WithOne(i => i.Purchase)
            .HasForeignKey(i => i.PurchaseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PurchaseItem>()
            .HasOne(i => i.InventoryItem)
            .WithMany()
            .HasForeignKey(i => i.InventoryItemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PurchaseItem>()
            .Property(i => i.UnitCost)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<PurchaseItem>()
            .Property(i => i.DiscountPercent)
            .HasColumnType("decimal(5,2)");

        modelBuilder.Entity<PurchaseItem>()
            .Property(i => i.IvaRate)
            .HasColumnType("decimal(5,2)");

        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Customer)
            .WithMany()
            .HasForeignKey(i => i.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.WorkOrder)
            .WithMany()
            .HasForeignKey(i => i.WorkOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Invoice>()
            .Property(i => i.LaborCost)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<Invoice>()
            .HasMany(i => i.Items)
            .WithOne(it => it.Invoice)
            .HasForeignKey(it => it.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InvoiceItem>()
            .HasOne(it => it.InventoryItem)
            .WithMany()
            .HasForeignKey(it => it.InventoryItemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InvoiceItem>()
            .Property(it => it.UnitPrice)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<InvoiceItem>()
            .Property(it => it.DiscountPercent)
            .HasColumnType("decimal(5,2)");

        modelBuilder.Entity<InvoiceItem>()
            .Property(it => it.IvaRate)
            .HasColumnType("decimal(5,2)");
        // Fuerza a que TODAS las propiedades DateTime se traten como UTC,
        // tanto al guardar como al leer de vuelta desde SQL Server.
        var utcConverter = new ValueConverter<DateTime, DateTime>(
            v => v,
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        var nullableUtcConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(utcConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(nullableUtcConverter);
                }
            }
        }
    }
}