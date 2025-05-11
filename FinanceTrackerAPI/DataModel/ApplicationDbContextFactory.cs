using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FinanceTrackerAPI.DataModel
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<FinanceTrackerAPIContext>
    {
        public FinanceTrackerAPIContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FinanceTrackerAPIContext>();
            // optionsBuilder.UseSqlServer("Server=localhost;Database=FinanceTrackerAPI;User Id=sa;Password=Zxvsm199#;");
            optionsBuilder.UseSqlServer("Server=localhost;Database=Finance Tracker API;User Id=sa;Password=Zxvsm199#;Trusted_Connection=True;ConnectRetryCount=2;TrustServerCertificate=true",
     options => options.EnableRetryOnFailure());
            return new FinanceTrackerAPIContext(optionsBuilder.Options);
        }
    }
}
