using Microsoft.EntityFrameworkCore;
using UrlShorter.Models;

namespace UrlShorter.DbAccess
{
    public class ApiDbContext: DbContext
    {
        public DbSet<UrlManager> Urls {  get; set; }
        public ApiDbContext(DbContextOptions options): base(options)
        {
            
        }
    }
}
