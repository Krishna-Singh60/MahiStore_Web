using MahiStore_Web.Data;
using MahiStore_Web.Models;
using MahiStore_Web.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public IActionResult Index(int pageIndex, string? search, string? column, string? orderBy)
        {
            IQueryable<Product> query = _dbContext.Products.AsNoTracking(); //Query the data from DB
            if (!string.IsNullOrWhiteSpace(search))  // If no search then it will not fail
            {
                query = query.Where(p =>
                        EF.Functions.Like(p.Name, $"%{search}%") ||
                        EF.Functions.Like(p.Brand, $"%{search}%"));
            }
            //sort funtionally
            string[] validColumn = {"Id","Name","Brand", "Category", "Price", "CreatedOn" };
            string[] validOrderBy = { "desc", "asc" };

            if(!validColumn.Contains(column, StringComparer.OrdinalIgnoreCase))
            {
                column = "Id";
            }

            if(!validOrderBy.Contains(orderBy, StringComparer.OrdinalIgnoreCase))
                {
                    orderBy = "asc";
            }

            query = (column?.ToLower(), orderBy?.ToLower()) switch
            {
                ("name", "asc") => query.OrderBy(p => p.Name),
                ("name", "desc") => query.OrderByDescending(p => p.Name),

                ("brand", "asc") => query.OrderBy(p => p.Brand),
                ("brand", "desc") => query.OrderByDescending(p => p.Brand),

                ("category", "asc") => query.OrderBy(p => p.Category),
                ("category", "desc") => query.OrderByDescending(p => p.Category),

                ("price", "asc") => query.OrderBy(p => p.Price),
                ("price", "desc") => query.OrderByDescending(p => p.Price),

                ("createdon", "asc") => query.OrderBy(p => p.CreatedAt),
                ("createdon", "desc") => query.OrderByDescending(p => p.CreatedAt),

                _ => query.OrderBy(p => p.Id)
            };
            //if (column == "Name")
            //{ 
            //    if (orderBy == "asc")
            //    {
            //        query = query.OrderBy(p => p.Name);
            //    }
            //    else
            //    {
            //        query = query.OrderByDescending(p => p.Name);
            //    }
            //}
            //else if (column == "Brand")
            //{
            //    if (orderBy == "asc")
            //    {
            //        query = query.OrderBy(p => p.Brand);
            //    }
            //    else
            //    {
            //        query = query.OrderByDescending(p => p.Brand);
            //    }
            //}
            //else if (column == "Category")
            //{
            //    if (orderBy == "asc")
            //    {
            //        query = query.OrderBy(p => p.Category);
            //    }
            //    else
            //    {
            //        query = query.OrderByDescending(p => p.Category);
            //    }
            //}
            //else if (column == "Price")
            //{
            //    if (orderBy == "asc")
            //    {
            //        query = query.OrderBy(p => p.Price);
            //    }
            //    else
            //    {
            //        query = query.OrderByDescending(p => p.Price);
            //    }
            //}
            //else if (column == "CreatedOn")
            //{
            //    if (orderBy == "asc")
            //    {
            //        query = query.OrderBy(p => p.CreatedAt);
            //    }
            //    else
            //    {
            //        query = query.OrderByDescending(p => p.CreatedAt);
            //    }
            //}
            //query = query.OrderBy(p => p.Id);  //After search we will order the data by Id in Ascending order
            int totalItems = query.Count();  //1st DB Call
            int totalPages = (int)Math.Ceiling((double)totalItems / _pageSize);

            if (pageIndex < 1)          { pageIndex = 1; }
            if (pageIndex > totalPages) { pageIndex = totalPages; }
            if (totalPages == 0)        { totalPages = 1; }

            var products  = query.Skip((pageIndex -1) * _pageSize)
                                .Take(_pageSize).ToList(); //2nd DB Call
            ViewData["CurrentPage"] = pageIndex;
            ViewData["TotalPages"] = totalPages;
            ViewData["Search"] = search ?? "";
            ViewData["Column"] = column;
            ViewData["OrderBy"] = orderBy;
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
