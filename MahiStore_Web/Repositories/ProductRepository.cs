using MahiStore_Web.Data;
using MahiStore_Web.Models;
using MahiStore_Web.Repositories.Interfaces;

namespace MahiStore_Web.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly ILogger<ProductRepository> _logger;
        public ProductRepository(ApplicationDBContext dBContext, ILogger<ProductRepository> logger)
        {
              _dbContext = dBContext;
              _logger = logger;
        }

        public bool Add(Product product)
        {
            try
            {
                if(product == null)
                {
                    throw new ArgumentNullException(nameof(product), "Product cannot be null.");
                }

                _dbContext.Products.Add(product);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                _logger.LogError(ex, "An error occurred while adding the product.");
                return false;
            }
        }

        public bool Delete(Product product)
        {
            try
            { 
                if(product == null)
                {
                    throw new ArgumentNullException(nameof(product), "Product cannot be null.");
                }
                 _dbContext.Products.Remove(product);
                 return true;
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                _logger.LogError(ex, "An error occurred while deleting the product.");
                return false;
            }
        }

        public Product? GetById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return null;
                }
                
                return _dbContext.Products.FirstOrDefault(p => p.Id == id);
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                Console.WriteLine($"An error occurred while retrieving the product: {ex.Message}");
                return null;
            }
        }

        public IQueryable<Product> GetProducts()
        {
            try
            {
                return _dbContext.Products.AsQueryable();
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                _logger.LogError(ex, "An error occurred while retrieving products.");
                return Enumerable.Empty<Product>().AsQueryable();
            }
        }

        public bool Save()
        {
            try
            {
                return _dbContext.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes to database.");
                return false;
            }
        }

        public bool Update(Product product)
        {
            try
            {
                if (product == null)
                {
                    throw new ArgumentNullException(nameof(product), "Product cannot be null.");
                }
                if (!_dbContext.Products.Any(p => p.Id == product.Id))
                {
                    return false; // Product not found
                }
                _dbContext.Products.Update(product);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                _logger.LogError(ex, "An error occurred while updating the product.");
                return false;
            }
            }
    }
}
