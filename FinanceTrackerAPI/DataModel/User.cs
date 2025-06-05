namespace FinanceTrackerAPI.DataModel
{
    using System.ComponentModel.DataAnnotations;

    public class User
    {
        public int Id { get; set; }
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string PasswordHash { get; set; }
        public required List<Category> Categories { get; set; } = new List<Category>();
    }
}
