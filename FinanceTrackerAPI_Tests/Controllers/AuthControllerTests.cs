using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FinanceTrackerAPI;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FinanceTrackerAPI.DataModel;
using FinanceTrackerAPI.Services;
using Microsoft.AspNetCore.Hosting;
namespace FinanceTrackerAPI.Controllers.Tests
{
    [TestClass]
    public class AuthControllerTests:TestBase
    {

        public AuthControllerTests() : base("TestDatabaseAuth")
        {
        }

        [TestMethod]
        public async Task Register_Success()
        {
            var dto = new RegisterDto { Email = "test@example.com", Password = "password" };
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            var response = await Client.PostAsync("/api/Auth/register", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<dynamic>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var token = ((JsonElement)result).GetProperty("token").GetString();
            Assert.IsNotNull(token);
            //Thread.Sleep(2000); // Wait for the in-memory database to update
            using var scope = ScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FinanceTrackerAPIContext>();
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
            Assert.IsNotNull(user);
            Assert.IsTrue(BCrypt.Net.BCrypt.Verify("password", user.PasswordHash));
            Assert.IsNotNull(user.Categories);
        }
        [TestMethod]
        public async Task Login_Success()
        {
            // Seed user
            using var scope = ScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FinanceTrackerAPIContext>();
            dbContext.Users.Add(new User
            {
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
                Categories = new List<Category>()
            });
            await dbContext.SaveChangesAsync();

            var dto = new LoginDto { Email = "test@example.com", Password = "password" };
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            var response = await Client.PostAsync("/api/Auth/login", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<dynamic>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var token = ((JsonElement)result).GetProperty("token").GetString();
            Assert.IsNotNull(token);
        }
        [TestMethod]
        public async Task Login_InvalidCredentials_Fails()
        {
            // Seed user
            using var scope = ScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FinanceTrackerAPIContext>();
            dbContext.Users.Add(new User
            {
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
                Categories = new List<Category>()
            });
            await dbContext.SaveChangesAsync();

            var dto = new LoginDto { Email = "test@example.com", Password = "wrong" };
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            var response = await Client.PostAsync("/api/Auth/login", content);
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

    }
/*    [TestClass]
    public class LoginSuccessTest
    {
        private static WebApplicationFactory<Program> _factory;
        private HttpClient _client;
        private IServiceScopeFactory _scopeFactory;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _factory = new WebApplicationFactory<Program>()
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

                        // Add in-memory DbContext
                        services.AddDbContext<FinanceTrackerAPIContext>(options =>
                            options.UseInMemoryDatabase("TestDatabaseAuth"));

                        // Add required services
                        services.AddScoped<IAuthService, AuthService>();
                    });
                });
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _factory?.Dispose();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _client = _factory.CreateClient();
            _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
            // Clear database
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FinanceTrackerAPIContext>();
            dbContext.Users.RemoveRange(dbContext.Users);
            dbContext.SaveChanges();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _client?.Dispose();
        }

        [TestMethod]
        public async Task Login_Success()
        {
            // Seed user
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FinanceTrackerAPIContext>();
            dbContext.Users.Add(new User
            {
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
                Categories = new List<Category>()
            });
            await dbContext.SaveChangesAsync();

            var dto = new LoginDto { Email = "test@example.com", Password = "password" };
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/Auth/login", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<dynamic>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var token = ((JsonElement)result).GetProperty("token").GetString();
            Assert.IsNotNull(token);
        }
    }
    [TestClass]
    public class LoginInvalidCredentialsTest
    {
        private static WebApplicationFactory<Program> _factory;
        private HttpClient _client;
        private IServiceScopeFactory _scopeFactory;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _factory = new WebApplicationFactory<Program>()
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

                        // Add in-memory DbContext
                        services.AddDbContext<FinanceTrackerAPIContext>(options =>
                            options.UseInMemoryDatabase("TestDatabaseAuth"));

                        // Add required services
                        services.AddScoped<IAuthService, AuthService>();
                    });
                });
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _factory?.Dispose();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _client = _factory.CreateClient();
            _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
            // Clear database
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FinanceTrackerAPIContext>();
            dbContext.Users.RemoveRange(dbContext.Users);
            dbContext.SaveChanges();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _client?.Dispose();
        }

        [TestMethod]
        public async Task Login_InvalidCredentials_Fails()
        {
            // Seed user
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FinanceTrackerAPIContext>();
            dbContext.Users.Add(new User
            {
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
                Categories = new List<Category>()
            });
            await dbContext.SaveChangesAsync();

            var dto = new LoginDto { Email = "test@example.com", Password = "wrong" };
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/Auth/login", content);
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }
    }*/
}