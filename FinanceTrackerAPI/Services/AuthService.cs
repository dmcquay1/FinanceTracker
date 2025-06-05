namespace FinanceTrackerAPI.Services
{
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using FinanceTrackerAPI.DataModel;

    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto dto);
        Task<string> LoginAsync(LoginDto dto);
    }

    public class AuthService : IAuthService
    {
        private readonly FinanceTrackerAPIContext _context;
        private readonly IConfiguration _config;

        public AuthService(FinanceTrackerAPIContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                throw new BadHttpRequestException("Email already exists");
 

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Categories = new List<Category>() // Initialize with an empty list
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            return GenerateJwtToken(user);
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new Exception("Invalid credentials");

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) },
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class RegisterDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class LoginDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class AuthResponse
    {
        public string Token { get; set; }
    }
}
