namespace FirstApi.Business.Services
{
    public interface ISignatureService
    {
        string Sign(string contentHash);
        bool VerifySignature(string contentHash, string signature);
    }
}