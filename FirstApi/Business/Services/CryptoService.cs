using System.Security.Cryptography;

namespace FirstApi.Business.Services
{
    public class CryptoService
    {
        private readonly byte[] _key;

        public CryptoService(IConfiguration configuration)
        {
            var keyString = configuration["Crypto:Key"];
            if (string.IsNullOrEmpty(keyString))
            {
                throw new InvalidOperationException("Crypto:Key bulunamadı. User Secrets kontrol et.");
            }
            _key = System.Text.Encoding.UTF8.GetBytes(keyString);
        }

        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.GenerateIV(); // her şifrelemede rastgele IV

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();

            // IV'yi başa yaz, decrypt ederken okuyacağız
            ms.Write(aes.IV, 0, aes.IV.Length);

            using var cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using var writer = new StreamWriter(cryptoStream);
            writer.Write(plainText);
            writer.Flush();
            cryptoStream.FlushFinalBlock();

            return Convert.ToBase64String(ms.ToArray());
        }

        public string Decrypt(string cipherTextWithIv)
        {
            var fullBytes = Convert.FromBase64String(cipherTextWithIv);

            using var aes = Aes.Create();
            aes.Key = _key;

            var iv = new byte[16]; // AES block size = 16 byte
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
    }
}