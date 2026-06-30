namespace FirstApi.Business.Services
{
    public interface ICryptoService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherTextWithIv);
    }
}