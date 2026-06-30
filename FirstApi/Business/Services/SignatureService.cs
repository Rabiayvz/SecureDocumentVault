using System.Security.Cryptography;

namespace FirstApi.Business.Services
{
    public class SignatureService : ISignatureService
    {
        private readonly RSA _privateRsa;
        private readonly RSA _publicRsa;
        private readonly ILogger<SignatureService> _logger;

        public SignatureService(IConfiguration configuration, ILogger<SignatureService> logger)
        {
            _logger = logger;

            string privateKeyPem;
            string publicKeyPem;

            var privateKeyPath = configuration["Signature:PrivateKeyPath"];
            var publicKeyPath = configuration["Signature:PublicKeyPath"];

            if (!string.IsNullOrEmpty(privateKeyPath) && File.Exists(privateKeyPath))
            {
                // Docker/dosya tabanlı okuma
                privateKeyPem = File.ReadAllText(privateKeyPath);
                publicKeyPem = File.ReadAllText(publicKeyPath!);
            }
            else
            {
                // Local development — User Secrets
                privateKeyPem = configuration["Signature:PrivateKey"] ?? "";
                publicKeyPem = configuration["Signature:PublicKey"] ?? "";
            }

            if (string.IsNullOrEmpty(privateKeyPem) || string.IsNullOrEmpty(publicKeyPem))
            {
                _logger.LogError("Signature keys not found.");
                throw new InvalidOperationException("Signature keys bulunamadı.");
            }

            _privateRsa = RSA.Create();
            _privateRsa.ImportFromPem(privateKeyPem);

            _publicRsa = RSA.Create();
            _publicRsa.ImportFromPem(publicKeyPem);
        }

        public string Sign(string contentHash)
        {
            try
            {
                var hashBytes = Convert.FromBase64String(contentHash);
                var signatureBytes = _privateRsa.SignHash(hashBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return Convert.ToBase64String(signatureBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Signing failed.");
                throw;
            }
        }

        public bool VerifySignature(string contentHash, string signature)
        {
            try
            {
                var hashBytes = Convert.FromBase64String(contentHash);
                var signatureBytes = Convert.FromBase64String(signature);
                return _publicRsa.VerifyHash(hashBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Signature verification failed.");
                return false;
            }
        }
    }
}