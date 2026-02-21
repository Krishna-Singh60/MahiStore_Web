using MahiStore_Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace MahiStore_Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDBContext _dbContext;
        public ProductsController(ApplicationDBContext dbContext)
        {
             _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            var products = _dbContext.Products.OrderByDescending(o=>o.Id).ToList();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
