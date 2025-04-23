using Microsoft.EntityFrameworkCore;
using Models;

namespace WebCrawler.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<CrawlerLogs> CrawlerLogs { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }

}
