namespace CcStore
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class PasswordHasher
    {
        private const int SaltSize = 16; // 128 bits
        private const int HashSize = 32; // 256 bits
        private const int Iterations = 10000;

        // Hash the password using PBKDF2
        public static string HashPassword(string password)
        {
            // Generate a salt
            using (var rng = new RNGCryptoServiceProvider())
            {
                var salt = new byte[SaltSize];
                rng.GetBytes(salt);

                // Hash the password with the salt using PBKDF2
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
                {
                    var hash = pbkdf2.GetBytes(HashSize);

                    // Combine the salt and hash into one string (salt:hash) to store in the database
                    var result = new byte[SaltSize + HashSize];
                    Array.Copy(salt, 0, result, 0, SaltSize);
                    Array.Copy(hash, 0, result, SaltSize, HashSize);

                    return Convert.ToBase64String(result);
                }
            }
        }

        // Verify the password against a stored hash
        public static bool VerifyPassword(string password, string storedHash)
        {
            // Convert the stored hash back into salt and hash parts
            var storedBytes = Convert.FromBase64String(storedHash);
            var salt = new byte[SaltSize];
            Array.Copy(storedBytes, 0, salt, 0, SaltSize);

            var storedPasswordHash = new byte[HashSize];
            Array.Copy(storedBytes, SaltSize, storedPasswordHash, 0, HashSize);

            // Hash the provided password with the stored salt
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                var passwordHash = pbkdf2.GetBytes(HashSize);

                // Compare the hashes
                for (int i = 0; i < HashSize; i++)
                {
                    if (passwordHash[i] != storedPasswordHash[i])
                        return false;
                }
            }

            return true;
        }
    }

}
