using FuneralOfficeSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FuneralOfficeSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<FuneralOffice> FuneralOffices { get; set; }
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
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<CategoryProperty> CategoryProperties { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductProperty> ProductProperties { get; set; }
        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<ServiceCategoryProperty> ServiceCategoryProperties { get; set; }
        public DbSet<ServiceProperty> ServiceProperties { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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

            // Ρύθμιση για την ιεραρχική δομή κατηγοριών
            builder.Entity<ProductCategory>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ρύθμιση σχέσεων για το Product
            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .HasMany(p => p.Inventories)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .HasMany(p => p.FuneralProducts)
                .WithOne(fp => fp.Product)
                .HasForeignKey(fp => fp.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .HasMany(p => p.Properties)
                .WithOne(pp => pp.Product)
                .HasForeignKey(pp => pp.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ρύθμιση σχέσεων για CategoryProperty
            builder.Entity<CategoryProperty>()
                .HasOne(cp => cp.Category)
                .WithMany(c => c.Properties)
                .HasForeignKey(cp => cp.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ρύθμιση για την ιεραρχική δομή κατηγοριών υπηρεσιών
            builder.Entity<ServiceCategory>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ρύθμιση σχέσεων για την Υπηρεσία
            builder.Entity<Service>()
                .HasOne(s => s.Category)
                .WithMany(c => c.Services)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Service>()
                .HasMany(s => s.Properties)
                .WithOne(sp => sp.Service)
                .HasForeignKey(sp => sp.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ρύθμιση σχέσεων για τις ιδιότητες κατηγορίας
            builder.Entity<ServiceCategoryProperty>()
                .HasOne(cp => cp.Category)
                .WithMany(c => c.Properties)
                .HasForeignKey(cp => cp.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}