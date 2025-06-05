using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Azure.Core.Pipeline;
///

namespace FinanceTrackerAPI.DataModel
{
    public class Category
    {
        public static Dictionary<int, Category>? cache ;
        public int CategoryId { get; set; }
        [Required(AllowEmptyStrings = false)]
        [MinLength(1, ErrorMessage = "Name must be at least 1 character.")]
        public string Name { get; set; } = string.Empty;
        public int? ParentCategoryId { get; set; } = null;

        // Navigation property
        [JsonIgnore]
        public Category? ParentCategory { get; set; } = null;
        [NotMapped]
        public List<Category> ChildCategories { get; set; } = new List<Category>();
        [NotMapped]
        public string FullCategoryPath => ComputeFullCategoryPath(this);
        [NotMapped]
        public int? Level => FullCategoryPath.Split(" > ").Count();
        public int UserId { get; set; }
        [JsonIgnore]
        public User? User { get; set; } = null;
        // Recursive method to compute the full path
        public static string ComputeFullCategoryPath(Category category)
        {
            if (category == null) return string.Empty;

            // Initialize cache if null
            cache ??= new Dictionary<int, Category>();

            // If already computed, return cached path
            if (cache.ContainsKey(category.CategoryId))
            {
                return cache[category.CategoryId].Name;
            }

            // Base case: Root category (no parent)
            if (!category.ParentCategoryId.HasValue || category.ParentCategory == null)
            {
                return category.Name;
            }

            // Recursive case: Get parent's path and append current category name
            var parentPath = ComputeFullCategoryPath(category.ParentCategory);
            var fullPath = $"{parentPath} > {category.Name}";

            // Cache the result
            return fullPath;
        }
    }

}
