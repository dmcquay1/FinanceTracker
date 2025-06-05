using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FinanceTrackerAPI;
using System.Net.Http;
using FinanceTrackerAPI.DataModel;
using FinanceTrackerAPI.Services;
using Microsoft.AspNetCore.Hosting;

public abstract class TestBase
{
    protected static WebApplicationFactory<Program> Factory { get; private set; }
    protected HttpClient Client { get; private set; }
    protected IServiceScopeFactory ScopeFactory { get; private set; }
    protected string DatabaseName { get; }

    protected TestBase(string databaseName)
    {
        DatabaseName = databaseName;
        
    }

    [ClassInitialize(InheritanceBehavior.BeforeEachDerivedClass)]
    public static void ClassInitialize(TestContext context)
    {
        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureServices(services =>
                {
                    // Remove all DbContext-related registrations
                    var dbContextDescriptors = services
                        .Where(d => d.ServiceType == typeof(DbContextOptions<FinanceTrackerAPIContext>) ||
                                    d.ServiceType == typeof(FinanceTrackerAPIContext) ||
                                    d.ServiceType.Name.Contains("DbContext"))
                        .ToList();
                    foreach (var descriptor in dbContextDescriptors)
                    {
                        services.Remove(descriptor);
                    }

                    // Add required services
                    services.AddScoped<IAuthService, AuthService>();
                });
            });
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        Factory?.Dispose();
    }

    [TestInitialize]
    public void TestInitialize()
    {
        // Configure DbContext with the specified database name
        Factory = Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddDbContext<FinanceTrackerAPIContext>(options =>
                    options.UseInMemoryDatabase(DatabaseName));
            });
        });

        Client = Factory.CreateClient();
        ScopeFactory = Factory.Services.GetRequiredService<IServiceScopeFactory>();
        // Clear database
        using var scope = ScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FinanceTrackerAPIContext>();
        dbContext.Users.RemoveRange(dbContext.Users);
        dbContext.Categories.RemoveRange(dbContext.Categories);
        dbContext.SaveChanges();
        SeedTestData();
    }

    [TestCleanup]
    public void TestCleanup()
    {
        Client?.Dispose();
    }

    protected virtual void SeedTestData()
    {
        // Default seeding (e.g., for auth tests)
        using var scope = ScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FinanceTrackerAPIContext>();
        dbContext.Database.EnsureCreated();
    }
}
