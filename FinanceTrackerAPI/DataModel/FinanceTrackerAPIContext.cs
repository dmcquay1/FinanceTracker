using FinanceTrackerAPI.DataModel;
using Microsoft.EntityFrameworkCore;

//namespace FinanceTrackerAPI.DataModel
//{
    public class FinanceTrackerAPIContext : DbContext
    {
        public FinanceTrackerAPIContext(DbContextOptions<FinanceTrackerAPIContext> options)
            : base(options)
        {
        }
        public DbSet<Transaction> Transactions { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>()
            .HasKey(t => t.TransactionId);
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId);

            // Configure the relationship
            entity.HasOne(e => e.ParentCategory)
                  .WithMany()
                  .HasForeignKey(e => e.ParentCategoryId)
                  .IsRequired(false); // ParentCategoryId is nullable

            // Ignore the computed property
            entity.Ignore(e => e.FullCategoryPath);
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
//}

