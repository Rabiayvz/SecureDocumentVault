using FirstApi.Models;

namespace FirstApi.Repositories
{
    public interface IDocumentRepository
    {
        Document? GetById(Guid id);
        IQueryable<Document> Query();
        void Add(Document document);
        void SaveChanges();
    }
}