using MahiStore_Web.Models;
using MahiStore_Web.Models.DTO;
using MahiStore_Web.Repositories;
using MahiStore_Web.Repositories.Interfaces;
using MahiStore_Web.Services.Interface;
using System.Linq.Expressions;

namespace MahiStore_Web.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepo, ILogger<ProductService> logger)
        {
           _productRepo = productRepo;
           _logger = logger;
        }

        public bool CreateProduct(ProductDTO dto, string imageFileName)
        {
            if(dto == null)
            {
                _logger.LogWarning("ProductDTO is null.");
                return false;
            }
            var product = new Product
            {
                Name = dto.Name,
                Brand = dto.Brand,
                Category = dto.Category,
                Price = dto.Price,
                Description = dto.Description,
                ImageFileName = imageFileName,
                CreatedAt = DateTime.Now
            };
            _productRepo.Add(product);
            return _productRepo.Save();
        }

        public bool DeleteProduct(int id)
        {
            try 
            { 
                if(id <=0)
                {
                    throw new ArgumentException("Product ID must be greater than zero.");
                }  
                var product = _productRepo.GetById(id);
                if(product == null)
                {
                    _logger.LogWarning($"Product with ID {id} not found for deletion.");
                    return false;
                }
                  _productRepo.Delete(product);
                return _productRepo.Save();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting product with ID {id}");
                return false;
            }
        }

        public Product? GetProduct(int id)
        {
            try 
            {
                if(id <=0)
                {
                    throw new ArgumentException("Product ID must be greater than zero.");
                }
                return _productRepo.GetById(id);
                 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching product with ID {id}");
                return null;
            }
        }

        public IQueryable<Product> GetProducts()
        {
            try
            { 
              return _productRepo.GetProducts();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products");
                throw;
            }
        }

        public bool UpdateProduct(int id, ProductDTO dto, string imageFileName)
        {
            try
            {
                if(id <=0)
                {
                    throw new ArgumentException("Product ID must be greater than zero.");
                }

                var existingProduct = _productRepo.GetById(id);

                if (existingProduct == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found", id);
                    return false;
                }

                existingProduct.Name = dto.Name;
                existingProduct.Brand = dto.Brand;
                existingProduct.Category = dto.Category;
                existingProduct.Price = dto.Price;
                existingProduct.Description = dto.Description;

                if (!string.IsNullOrEmpty(imageFileName))
                    existingProduct.ImageFileName = imageFileName;

                _productRepo.Update(existingProduct);

                return _productRepo.Save();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating product with ID {id}");
                return false;
            }
        }
    }
}
