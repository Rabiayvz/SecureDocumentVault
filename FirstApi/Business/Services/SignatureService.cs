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

            var privateKeyPem = configuration["Signature:PrivateKey"];
            var publicKeyPem = configuration["Signature:PublicKey"];

            if (string.IsNullOrEmpty(privateKeyPem) || string.IsNullOrEmpty(publicKeyPem))
            {
                _logger.LogError("Signature keys not found in configuration.");
                throw new InvalidOperationException("Signature:PrivateKey veya Signature:PublicKey bulunamadı.");
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