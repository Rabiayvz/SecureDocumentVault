using System.Security.Cryptography;

namespace FirstApi.Business.Services
{
    public class CryptoService
    {
        private readonly byte[] _key;
        private readonly ILogger<CryptoService> _logger;

        public CryptoService(IConfiguration configuration, ILogger<CryptoService> logger)
        {
            _logger = logger;

            var keyString = configuration["Crypto:Key"];
            if (string.IsNullOrEmpty(keyString))
            {
                _logger.LogError("Crypto:Key not found in configuration.");
                throw new InvalidOperationException("Crypto:Key bulunamadı. User Secrets kontrol et.");
            }
            _key = Convert.FromBase64String(keyString);
        }

        public string Encrypt(string plainText)
        {
            try
            {
                using var aes = Aes.Create();
                aes.Key = _key;
                aes.GenerateIV();

                using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using var ms = new MemoryStream();

                ms.Write(aes.IV, 0, aes.IV.Length);

                using var cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
                using var writer = new StreamWriter(cryptoStream);
                writer.Write(plainText);
                writer.Flush();
                cryptoStream.FlushFinalBlock();

                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Encryption failed.");
                throw;
            }
        }

        public string Decrypt(string cipherTextWithIv)
        {
            try
            {
                var fullBytes = Convert.FromBase64String(cipherTextWithIv);

                using var aes = Aes.Create();
                aes.Key = _key;

                var iv = new byte[16];
                Array.Copy(fullBytes, 0, iv, 0, iv.Length);
                aes.IV = iv;

                var cipherBytes = new byte[fullBytes.Length - iv.Length];
                Array.Copy(fullBytes, iv.Length, cipherBytes, 0, cipherBytes.Length);

                using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using var ms = new MemoryStream(cipherBytes);
                using var cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var reader = new StreamReader(cryptoStream);

                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Decryption failed.");
                throw;
            }
        }
    }
}