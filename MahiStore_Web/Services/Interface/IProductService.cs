using MahiStore_Web.Models;
using MahiStore_Web.Models.DTO;

namespace MahiStore_Web.Services.Interface
{
    public interface IProductService
    {
        bool CreateProduct(ProductDTO dto, string imageFileName);
        bool UpdateProduct(int id, ProductDTO dto, string imageFileName);
        bool DeleteProduct(int id);
        Product? GetProduct(int id);
        IQueryable<Product> GetProducts();
    }
}
