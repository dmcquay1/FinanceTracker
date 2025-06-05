using FinanceTrackerAPI.DataModel;
using Microsoft.EntityFrameworkCore;

namespace FinanceTrackerAPI.DataModel
{
    public class FinanceTrackerAPIContext : DbContext
    {
        public FinanceTrackerAPIContext(DbContextOptions<FinanceTrackerAPIContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Transaction>()
                .HasKey(t => t.TransactionId);
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId);

                // Configure the relationship
                entity.HasOne(e => e.ParentCategory)
                      .WithMany(e => e.ChildCategories)
                      .HasForeignKey(e => e.ParentCategoryId)
                      .OnDelete(DeleteBehavior.Restrict) // Prevent cascading delete
                      .IsRequired(false); // ParentCategoryId is nullable
                entity.Property(e => e.Name).IsRequired(true);
                      
                // Ignore the computed property
                entity.Ignore(e => e.FullCategoryPath);

                entity.HasOne(e => e.User)
                .WithMany(u => u.Categories)
                        .HasForeignKey(e => e.UserId);
            });
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                optionsBuilder.UseSqlServer("Server=localhost;Database=Finance Tracker API;User Id=sa;Password=Zxvsm199#;Trusted_Connection=True;ConnectRetryCount=2;TrustServerCertificate=true",
                    options => options.EnableRetryOnFailure());

            }
        }
    }
}

