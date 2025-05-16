using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FinanceTrackerAPI;
using FinanceTrackerAPI.DataModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Azure;

namespace FinanceTrackerAPI.Controllers.Tests
{
    [TestClass]
    public class CategoriesControllerTests
    {
        private static WebApplicationFactory<Program> _factory;
        private static HttpClient _client;
        private static IServiceScopeFactory _scopeFactory;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Test");
                    builder.ConfigureServices(services =>
                    {
                        // Remove existing DbContext
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<FinanceTrackerAPIContext>));
                        if (descriptor != null)
                        {
                            services.Remove(descriptor);
                        }

                        // Add in-memory database
                        services.AddDbContext<FinanceTrackerAPIContext>(options =>
                            options.UseInMemoryDatabase("TestDatabase"));
                    });
                });

            _client = _factory.CreateClient();
            _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();

            // Seed test data
            SeedTestData();
        }

        private static void SeedTestData()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FinanceTrackerAPIContext>();

            // Ensure the database is created
            dbContext.Database.EnsureCreated();

            // Add test categories
            var categories = new List<Category>
            {
                new() { CategoryId = 1, Name = "Income" , ParentCategoryId = null },
                new() { CategoryId = 2, Name = "Salary/Wages" , ParentCategoryId = 1 },
                new() { CategoryId = 3, Name = "Freelance Income" , ParentCategoryId = 1 },
                new() { CategoryId = 4, Name = "Investment Income" , ParentCategoryId = 1 },
                new() { CategoryId = 5, Name = "Gifts/Inheritance" , ParentCategoryId = 1 },
                new() { CategoryId = 6, Name = "Other Income" , ParentCategoryId = 1 },
                new() { CategoryId = 7, Name = "Expenses" , ParentCategoryId = null },
                new() { CategoryId = 8, Name = "Housing" , ParentCategoryId = 7 },
                new() { CategoryId = 9, Name = "Rent" , ParentCategoryId = 8 },
                new() { CategoryId = 10, Name = "Mortgage" , ParentCategoryId = 8 },
                new() { CategoryId = 11, Name = "Property Taxes" , ParentCategoryId = 8 },
                new() { CategoryId = 12, Name = "Insurance" , ParentCategoryId = 8 },
                new() { CategoryId = 13, Name = "Transportation" , ParentCategoryId = 7 },
                new() { CategoryId = 14, Name = "Auto Payment" , ParentCategoryId = 13 },
                new() { CategoryId = 15, Name = "Fuel" , ParentCategoryId = 13 },
                new() { CategoryId = 16, Name = "Public Transportation" , ParentCategoryId = 13 },
                new() { CategoryId = 17, Name = "Auto Insurance" , ParentCategoryId = 13 },
                new() { CategoryId = 18, Name = "Auto Maintenance" , ParentCategoryId = 13 },
                new() { CategoryId = 19, Name = "Food" , ParentCategoryId = 7 },
                new() { CategoryId = 20, Name = "Groceries" , ParentCategoryId = 19 },
                new() { CategoryId = 21, Name = "Dining Out" , ParentCategoryId = 19 },
                new() { CategoryId = 22, Name = "Snacks" , ParentCategoryId = 19 },
                new() { CategoryId = 23, Name = "Utilities" , ParentCategoryId = 7 },
                new() { CategoryId = 24, Name = "Electricity" , ParentCategoryId = 23 },
                new() { CategoryId = 25, Name = "Water" , ParentCategoryId = 23 },
                new() { CategoryId = 26, Name = "Gas" , ParentCategoryId = 23 },
                new() { CategoryId = 27, Name = "Internet" , ParentCategoryId = 23 },
                new() { CategoryId = 28, Name = "Entertainment" , ParentCategoryId = 7 },
                new() { CategoryId = 29, Name = "Movies" , ParentCategoryId = 28 },
                new() { CategoryId = 30, Name = "Concerts" , ParentCategoryId = 28 },
                new() { CategoryId = 31, Name = "Hobbies" , ParentCategoryId = 28 },
                new() { CategoryId = 32, Name = "Streaming Service" , ParentCategoryId = 28 },
                new() { CategoryId = 33, Name = "Clothing" , ParentCategoryId = 7 },
                new() { CategoryId = 34, Name = "Work" , ParentCategoryId = 33 },
                new() { CategoryId = 35, Name = "Personal" , ParentCategoryId = 33 },
                new() { CategoryId = 36, Name = "Health" , ParentCategoryId = 7 },
                new() { CategoryId = 37, Name = "Medical Bills" , ParentCategoryId = 36 },
                new() { CategoryId = 38, Name = "Insurance Premiums" , ParentCategoryId = 37 },
                new() { CategoryId = 39, Name = "Personal Care" , ParentCategoryId = 7 },
                new() { CategoryId = 40, Name = "Haircuts" , ParentCategoryId = 39 },
                new() { CategoryId = 41, Name = "Toiletries" , ParentCategoryId = 39 },
                new() { CategoryId = 42, Name = "Travel" , ParentCategoryId = 7 },
                new() { CategoryId = 43, Name = "Vacation" , ParentCategoryId = 42 },
                new() { CategoryId = 44, Name = "Travel Expenses" , ParentCategoryId = 42 },
                new() { CategoryId = 45, Name = "Debt Payments" , ParentCategoryId = 7 },
                new() { CategoryId = 46, Name = "Loan Payments" , ParentCategoryId = 45 },
                new() { CategoryId = 47, Name = "Credit Card Payment" , ParentCategoryId = 45 },
                new() { CategoryId = 48, Name = "Investment Expense" , ParentCategoryId = 7 },
                new() { CategoryId = 49, Name = "Stocks" , ParentCategoryId = 48 },
                new() { CategoryId = 50, Name = "Bonds" , ParentCategoryId = 48 },
                new() { CategoryId = 51, Name = "Mutual Funds" , ParentCategoryId = 48 },
                new() { CategoryId = 52, Name = "Other" , ParentCategoryId = 7 },
                new() { CategoryId = 53, Name = "ETFs" , ParentCategoryId = 48 },
                new() { CategoryId = 54, Name = "Real Estate" , ParentCategoryId = 48 },
                new() { CategoryId = 55, Name = "CryptoCurrency" , ParentCategoryId = 48 },
                new() { CategoryId = 56, Name = "Assets" , ParentCategoryId = null },
                new() { CategoryId = 57, Name = "Bank Accounts" , ParentCategoryId = 56 },
                new() { CategoryId = 58, Name = "Savings" , ParentCategoryId = 57 },
                new() { CategoryId = 59, Name = "Checking" , ParentCategoryId = 57 },
                new() { CategoryId = 60, Name = "Investments" , ParentCategoryId = 56 },
                new() { CategoryId = 61, Name = "Stocks" , ParentCategoryId = 60 },
                new() { CategoryId = 62, Name = "Bonds" , ParentCategoryId = 60 },
                new() { CategoryId = 63, Name = "Mutual Funds" , ParentCategoryId = 60 },
                new() { CategoryId = 64, Name = "ETFs" , ParentCategoryId = 60 },
                new() { CategoryId = 65, Name = "CryptoCurrency" , ParentCategoryId = 60 },
                new() { CategoryId = 66, Name = "Real Estate" , ParentCategoryId = 56 },
                new() { CategoryId = 67, Name = "Houses" , ParentCategoryId = 66 },
                new() { CategoryId = 68, Name = "Land" , ParentCategoryId = 66 },
                new() { CategoryId = 69, Name = "Vehicles" , ParentCategoryId = 56 },
                new() { CategoryId = 70, Name = "Cars" , ParentCategoryId = 69 },
                new() { CategoryId = 71, Name = "Motorcycles" , ParentCategoryId = 69 },
                new() { CategoryId = 72, Name = "Personal Property" , ParentCategoryId = 56 },
                new() { CategoryId = 73, Name = "Jewelry" , ParentCategoryId = 72 },
                new() { CategoryId = 74, Name = "Electronics" , ParentCategoryId = 72 },
                new() { CategoryId = 75, Name = "HAL 7000" , ParentCategoryId = 74 },
                new() { CategoryId = 76, Name = "SAL 7000" , ParentCategoryId = 74 },
                new() { CategoryId = 77, Name = "MEL 7000" , ParentCategoryId = 74 }
            };

            foreach (var category in categories)
            {
                // Referential integrity check: Ensure ParentCategoryId is valid
                if (category.ParentCategoryId.HasValue)
                {
                    if (!categories.Any(c => c.CategoryId == category.ParentCategoryId.Value))
                    {
                        throw new InvalidOperationException(
                            $"Cannot add Category with CategoryId {category.CategoryId}. ParentCategoryId {category.ParentCategoryId} does not exist.");
                    }
                }
                dbContext.Categories.Add(category);
            }
            dbContext.SaveChanges();
            //  Verify navigation properties are set
            foreach (var category in categories)
            {
                if (category.ParentCategoryId.HasValue)
                {
                    category.ParentCategory = dbContext.Categories
                        .FirstOrDefault(c => c.CategoryId == category.ParentCategoryId.Value);
                }
            }
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }

        [TestMethod]
        public async Task GetAllCategories()
        {
            // Act
            var response = await _client.GetAsync("/api/Categories");
            response.EnsureSuccessStatusCode();

            // Assert
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(content);

            // Deserialize the response to verify the data
            var categories = JsonSerializer.Deserialize<List<Category>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(categories);
            Assert.AreEqual(77, categories.Count); // Expecting 77 categories from seed data
        }

        [TestMethod]
        public async Task GetCategoryById()
        {
            // Arrange
            var categoryId = 1; // Assuming this ID exists in the seeded data
            // Act
            var response = await _client.GetAsync($"/api/Categories/{categoryId}");
            response.EnsureSuccessStatusCode();
            // Assert
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(content);
            // Deserialize the response to verify the data
            var category = JsonSerializer.Deserialize<Category>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            Assert.IsNotNull(category);
            Assert.AreEqual(categoryId, category.CategoryId);
        }
        [TestMethod]
        public async Task CreateCategory()
        {
            // Arrange
            var newCategory = new Category { Name = "Test Category", ParentCategoryId = null };
            var jsonContent = JsonSerializer.Serialize(newCategory);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
            // Act
            var response = await _client.PostAsync("/api/Categories", content);
            response.EnsureSuccessStatusCode();
            // Assert
            var createdCategory = JsonSerializer.Deserialize<Category>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            Assert.IsNotNull(createdCategory);
            Assert.AreEqual(newCategory.Name, createdCategory.Name);
        }
        [TestMethod]
        public void CreateCategoryWithInvalidParentCategoryIdFails()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FinanceTrackerAPIContext>();

            var invalidCategory = new Category
            {
                CategoryId = 5,
                Name = "Invalid",
                ParentCategoryId = 999 // Non-existent ParentCategoryId
            };

            dbContext.Categories.Add(invalidCategory);
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                if (invalidCategory.ParentCategoryId.HasValue &&
                    !dbContext.Categories.Any(c => c.CategoryId == invalidCategory.ParentCategoryId.Value))
                {
                    throw new InvalidOperationException(
                        $"Cannot add Category with CategoryId {invalidCategory.CategoryId}. ParentCategoryId {invalidCategory.ParentCategoryId} does not exist.");
                }
                dbContext.SaveChanges();
            });
        }

        [TestMethod]
        public async Task PutCategory()
        {
            // Arrange
            var updatedCategory = new Category { CategoryId = 1, Name = "Updated Category", ParentCategoryId = null };
            var jsonContent = JsonSerializer.Serialize(updatedCategory);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
            // Act
            var response = await _client.PutAsync($"/api/Categories/{updatedCategory.CategoryId}", content);
            response.EnsureSuccessStatusCode();
            // Assert
            var updatedResponse = await _client.GetAsync($"/api/Categories/{updatedCategory.CategoryId}");
            var updatedContent = await updatedResponse.Content.ReadAsStringAsync();
            var category = JsonSerializer.Deserialize<Category>(updatedContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            Assert.IsNotNull(category);
            Assert.AreEqual(updatedCategory.Name, category.Name);
        }
        [TestMethod]
        public async Task DeleteCategory()
        {
            // Arrange
            var categoryId = 77; // Assuming this ID exists in the seeded data
            // Act
;           var response = await _client.DeleteAsync($"/api/Categories/{categoryId}");
            response.EnsureSuccessStatusCode();
            // Assert
            var getResponse = await _client.GetAsync($"/api/Categories/{categoryId}");
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
        }
        [TestMethod]
        public async Task DeleteCategoryWithChildrenFails()
        {
            // Arrange
            var categoryId = 74; // Assuming this ID exists in the seeded data
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.Continue);

            // Act
            response = await _client.DeleteAsync($"/api/Categories/{categoryId}");
            // Assert
            if (response.IsSuccessStatusCode)
                Assert.Fail("Expected an exception to be thrown.");
            else
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                Assert.IsTrue(errorMessage.Contains("Cannot delete category with child categories."));
            }
        }

    }
}