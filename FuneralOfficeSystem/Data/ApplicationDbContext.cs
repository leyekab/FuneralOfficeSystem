using FuneralOfficeSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FuneralOfficeSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<FuneralOffice> FuneralOffices { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Deceased> Deceaseds { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Funeral> Funerals { get; set; }
        public DbSet<FuneralProduct> FuneralProducts { get; set; }
        public DbSet<FuneralService> FuneralServices { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<BurialPlace> BurialPlaces { get; set; }
        public DbSet<Church> Churches { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // SQLite δεν υποστηρίζει κάποιους τύπους που χρησιμοποιεί το EF Core από προεπιλογή
            // Προσαρμόζουμε τους τύπους δεδομένων για να λειτουργούν με SQLite

            // Relationships/constraints configuration
            builder.Entity<FuneralProduct>()
                .HasOne(fp => fp.Funeral)
                .WithMany(f => f.FuneralProducts)
                .HasForeignKey(fp => fp.FuneralId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<FuneralProduct>()
                .HasOne(fp => fp.Product)
                .WithMany(p => p.FuneralProducts)
                .HasForeignKey(fp => fp.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FuneralService>()
                .HasOne(fs => fs.Funeral)
                .WithMany(f => f.FuneralServices)
                .HasForeignKey(fs => fs.FuneralId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<FuneralService>()
                .HasOne(fs => fs.Service)
                .WithMany(s => s.FuneralServices)
                .HasForeignKey(fs => fs.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Inventory>()
                .HasOne(i => i.FuneralOffice)
                .WithMany(fo => fo.Inventories)
                .HasForeignKey(i => i.FuneralOfficeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Inventory>()
                .HasOne(i => i.Product)
                .WithMany(p => p.Inventories)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Funeral>()
                .HasOne(f => f.FuneralOffice)
                .WithMany(fo => fo.Funerals)
                .HasForeignKey(f => f.FuneralOfficeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Funeral>()
                .HasOne(f => f.Deceased)
                .WithMany(d => d.Funerals)
                .HasForeignKey(f => f.DeceasedId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Funeral>()
                .HasOne(f => f.Client)
                .WithMany(c => c.Funerals)
                .HasForeignKey(f => f.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Funeral>()
                .HasOne(f => f.BurialPlace)
                .WithMany(c => c.Funerals)
                .HasForeignKey(f => f.BurialPlaceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Funeral>()
                .HasOne(f => f.Church)
                .WithMany(c => c.Funerals)
                .HasForeignKey(f => f.ChurchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .HasOne(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Service>()
                .HasOne(s => s.Supplier)
                .WithMany(s => s.Services)
                .HasForeignKey(s => s.SupplierId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<InventoryTransaction>()
                .HasOne(it => it.Product)
                .WithMany()
                .HasForeignKey(it => it.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<InventoryTransaction>()
                .HasOne(it => it.SourceFuneralOffice)
                .WithMany()
                .HasForeignKey(it => it.SourceFuneralOfficeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<InventoryTransaction>()
                .HasOne(it => it.DestinationFuneralOffice)
                .WithMany()
                .HasForeignKey(it => it.DestinationFuneralOfficeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}