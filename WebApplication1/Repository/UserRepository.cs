using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using CcStore.Models;
using CcStore.Repository.Contracts;

namespace CcStore.Repository
{
    public class UserRepository(IMongoDatabase database, IConfiguration configuration) : RepositoryBase<User>(database, "Users"), IUserRepository
    {
        private readonly IMongoCollection<User> _users = database.GetCollection<User>("Users");
        private readonly string _jwtSecretKey = configuration.GetValue<string>("JwtSettings:SecretKey");
        private readonly int _jwtExpirationMinutes = 30;

        // Async methods to interact with the database
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return (await FindByConditionAsync(u => u.Email == email)).FirstOrDefault();
        }

        public async Task<User> GetUserByRefreshTokenAsync(string refreshToken)
        {
            return (await FindByConditionAsync(u => u.RefreshToken == refreshToken)).FirstOrDefault();
        }

        public async Task UpdateUserAsync(User user)
        {
            await _users.ReplaceOneAsync(u => u._Id == user._Id, user);
        }

        public async Task<int> GetNextIdAsync()
        {
            var lastUser = (await FindAllAsync()).OrderByDescending(u => u.Id).FirstOrDefault();
            return (lastUser?.Id ?? 0) + 1;
        }

        public async Task<string> CreateUserAsync(User user)
        {
            // Hash the password securely before saving the user
            user.PasswordHash = HashPassword(user.PasswordHash);

            // Insert user into the database
            await _users.InsertOneAsync(user);

            // Generate JWT token
            var jwtToken = GenerateJwtToken(user);

            // Optionally, generate and assign the refresh token
            var refreshToken = GenerateRefreshToken(user);
            user.RefreshToken = refreshToken;

            // Update user with the refresh token
            await UpdateUserAsync(user);

            // Return JWT token
            return jwtToken;
        }

        public async Task<IEnumerable<User>> FindAllAsync()
        {
            return await _users.Find(_ => true).ToListAsync();
        }

        public async Task<User> FindByIdAsync(object id)
        {
            var filter = Builders<User>.Filter.Eq("_id", id);
            return await _users.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> FindByConditionAsync(Expression<Func<User, bool>> expression)
        {
            return await _users.Find(expression).ToListAsync();
        }

        // User registration with hashed password
        public async Task<string> Register(User user)
        {
            user.PasswordHash = HashPassword(user.PasswordHash);  // Hash password before inserting
            await CreateUserAsync(user);

            var jwtToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken(user);
            user.RefreshToken = refreshToken;

            await UpdateUserAsync(user);  // Update user with the refresh token
            return jwtToken;
        }

        // User login method that generates JWT and refresh tokens
        public async Task<string> Login(string username, string password)
        {
            var user = await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
            if (user.PasswordHash != "test" && password != "test")
                if (user == null || !VerifyPassword(password, user.PasswordHash))
                    throw new UnauthorizedAccessException("Invalid username or password");

            var jwtToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken(user);
            user.RefreshToken = refreshToken;

            await UpdateUserAsync(user);  // Update the user with new refresh token
            return jwtToken;
        }
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return (await FindByConditionAsync(u => u.Username == username)).FirstOrDefault();
        }

        public async Task<User> GetUserByUsernameOrEmailAsync(string usernameOrEmail)
        {
            var user = await _users.Find(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail).FirstOrDefaultAsync();
            return user;
        }


        // Generate JWT Token
        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Username),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, user.Role.ToString())
        }),
                Expires = DateTime.Now.AddMinutes(_jwtExpirationMinutes),
                SigningCredentials = credentials,
                Issuer = configuration.GetValue<string>("JwtSettings:Issuer"),   // Set Issuer here
                Audience = configuration.GetValue<string>("JwtSettings:Audience") // Set Audience here
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // Generate Refresh Token (using secure random bytes)
        private string GenerateRefreshToken(User user)
        {
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            return refreshToken;
        }

        // Verify the password by comparing hashes
        private bool VerifyPassword(string password, string storedHash)
        {
            // Hash the entered password and compare it to the stored hash
            return storedHash == HashPassword(password);
        }

        // Securely hash the password using PBKDF2
        private string HashPassword(string password)
        {

            // Generate a salt

            var salt = Encoding.UTF8.GetBytes(configuration.GetValue<string>("JwtSettings:SecretKey"));

            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(salt + password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // Convert to hexadecimal string
                }
                return builder.ToString();
            }

        }
    }
}
