using MahiStore_Web.Models;
using MahiStore_Web.Models.DTO;
using MahiStore_Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace MahiStore_Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly int _pageSize = 5;
        public ProductsController(ApplicationDBContext dbContext, IWebHostEnvironment environment)
        {
             _dbContext = dbContext;
            _environment = environment;
        }
        public IActionResult Index(int pageIndex)
        {
            //IQueryable<Product> query = _dbContext.Products;
            //var query = _dbContext.Products.OrderByDescending(o => o.Id);
            var query = _dbContext.Products.OrderBy(o => o.Id);
            int totalItems = query.Count();  //1st DB Call
            int totalPages = (int)Math.Ceiling((double)totalItems / _pageSize);

            if (pageIndex < 1)
            {
                pageIndex = 1;
            }

            if (pageIndex > totalPages) { pageIndex = totalPages; }

           
           var products  = query.Skip((pageIndex -1) * _pageSize)
                                .Take(_pageSize).ToList(); //2nd DB Call
            ViewData["CurrentPage"] = pageIndex;
            ViewData["TotalPages"] = totalPages;
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(ProductDTO productDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(productDTO);
            }
            if (productDTO.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "Please upload an image.");
                return View(productDTO);
            }

            //Save the file to wwwroot/images and get the path
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(productDTO.ImageFile.FileName);
            string filePath = Path.Combine(_environment.WebRootPath, "Productimages", newFileName);
            using (var stream = System.IO.File.Create(filePath))
            {
                productDTO.ImageFile.CopyTo(stream);
            }

            //Save the new Product in to DB
            Product product = new Product
            {
                Name = productDTO.Name,
                Brand = productDTO.Brand,
                Category = productDTO.Category,
                Price = productDTO.Price,
                Description = productDTO.Description,
                ImageFileName = newFileName,
                CreatedAt = DateTime.Now
            };

            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index), "Products");
            
        }

        public IActionResult Edit(int Id)
        { 
            var product = _dbContext.Products.Find (Id);
            if(product ==null)
            {
                return RedirectToAction(nameof(Index), "Products");
            }

            //Update the product details in DB
            var productDTO = new ProductDTO
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description
            };

            ViewData["ProductId"] = product.Id;
            ViewData["ImageFile"] = product.ImageFileName;
            ViewData["CreatedDate"] = Product.ReferenceEquals(product.CreatedAt, default(DateTime)) ? "N/A" : product.CreatedAt.ToString("dd-MM-yyyy");

            return View(productDTO);
        }

        [HttpPost]
        public IActionResult Edit(int Id, ProductDTO productDTO)
        {
            var product = _dbContext.Products.Find(Id);

            if (product == null)
            {
                return RedirectToAction(nameof(Index), "Products");
            }

            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = product.Id;
                ViewData["ImageFile"] = product.ImageFileName;
                ViewData["CreatedDate"] = Product.ReferenceEquals(product.CreatedAt, default(DateTime)) ? "N/A" : product.CreatedAt.ToString("dd-MM-yyyy");

                return View(productDTO);
            }

            //Update the image file if we have a new image file
            string newFileName = product.ImageFileName;
            if (productDTO.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(productDTO.ImageFile.FileName);
                string filePath = Path.Combine(_environment.WebRootPath, "Productimages", newFileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    productDTO.ImageFile.CopyTo(stream);
                }

                //Delete the old Image file
                string oldImageFile = _environment.WebRootPath + "Productimages" + product.ImageFileName;
                System.IO.File.Delete(oldImageFile);
            }

            product.Name = productDTO.Name;
            product.Brand = productDTO.Brand;
            product.Category = productDTO.Category;
            product.Price = productDTO.Price;
            product.Description = productDTO.Description;
            product.ImageFileName = newFileName;

            _dbContext.Products.Update(product);
            _dbContext.SaveChanges();


            return RedirectToAction(nameof(Index), "Products");
        }

        //[HttpGet]
        //public IActionResult Delete(int id)
        //{
        //    var product = _dbContext.Products.Find(id);
        //    if (product == null)
        //        return RedirectToAction(nameof(Index));

        //    return View(product);
        //}
      
        public IActionResult Delete(int Id)
        {
            var product = _dbContext.Products.Find(Id);
            if (product == null)
            {
                return RedirectToAction(nameof(Index), "Products");
            }

            //Delete the image file from wwwroot/images
            string filePath = Path.Combine(_environment.WebRootPath, "Productimages", product.ImageFileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            //Delete the Product from DB
            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index), "Products");

        }


    }
}
