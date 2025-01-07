using Bookly.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Data
{
    public class ApplicationDbContext : DbContext
    {
        //connection string
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options ): base(options) 
        {
                    
        }
        public  DbSet<Category> Category { get; set; }
    }
}
