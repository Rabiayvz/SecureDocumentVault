namespace FirstApi.Business.Services
{
    public interface IHashService
    {
        string ComputeHash(string content);
        bool VerifyHash(string content, string storedHash);
    }
}