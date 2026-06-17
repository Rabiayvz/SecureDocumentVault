using System.Security.Cryptography;
using System.Text;

namespace FirstApi.Business.Services
{
    public class HashService
    {
        public string ComputeHash(string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }

        public bool VerifyHash(string content, string storedHash)
        {
            var computedHash = ComputeHash(content);
            return computedHash == storedHash;
        }
    }
}