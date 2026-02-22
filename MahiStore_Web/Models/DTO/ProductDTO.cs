using System.ComponentModel.DataAnnotations;

namespace MahiStore_Web.Models.DTO
{
    public class ProductDTO
    {
        public string Name { get; set; }
        [Required,MaxLength(100)]
        public string Brand { get; set; }
        [Required,MaxLength(100)]
        public string Category { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string Description { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
