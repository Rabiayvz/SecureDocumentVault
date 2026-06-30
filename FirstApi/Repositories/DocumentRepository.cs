using FirstApi.Data;
using FirstApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstApi.Repositories
{
    public class DocumentRepository
    {
        private readonly AppDbContext _context;

        public DocumentRepository(AppDbContext context)
        {
            _context = context;
        }

        public Document? GetById(Guid id)
        {
            return _context.Documents.Find(id);
        }

        public IQueryable<Document> Query()
        {
            return _context.Documents;
        }

        public void Add(Document document)
        {
            _context.Documents.Add(document);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}