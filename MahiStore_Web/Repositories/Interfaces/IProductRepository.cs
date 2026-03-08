using MahiStore_Web.Models;

namespace MahiStore_Web.Repositories.Interfaces
{
    public interface IProductRepository
    {
        IQueryable<Product> GetProducts();
        Product GetById(int id);
        bool Add(Product product);
        bool Update(Product product);
        bool Delete(Product product);
        bool Save();
    }
}
