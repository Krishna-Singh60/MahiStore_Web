using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MahiStore_Web.Services

{
    //public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDBContext>
    //{
    //    public ApplicationDBContext CreateDbContext(string[] args)
    //    {
    //        // Go to project root from bin folder
    //        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..");

    //        IConfiguration config = new ConfigurationBuilder()
    //            .SetBasePath(basePath)
    //            .AddJsonFile("appsettings.json", optional: false)
    //            .Build();

    //        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDBContext>();

    //        var connectionString = config.GetConnectionString("DefaultConnection");

    //        optionsBuilder.UseSqlServer(connectionString);

    //        return new ApplicationDBContext(optionsBuilder.Options);
    //    }
    //}

}
