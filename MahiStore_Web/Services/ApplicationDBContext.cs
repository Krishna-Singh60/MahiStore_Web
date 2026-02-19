using MahiStore_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace MahiStore_Web.Services
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options) 
        {
                
        }

        public DbSet<Product> Products { get; set; }
    }
}
