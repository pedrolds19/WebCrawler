using Models;
using WebCrawler.Data;  
namespace Data.Home
{
    public class HomeDAL
    {
        private readonly ApplicationDbContext _context;

        public HomeDAL(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task GravarLog(CrawlerLogs log)
        {
            try
            {
                await _context.CrawlerLogs.AddAsync(log);
                await _context.SaveChangesAsync(); 

                Console.WriteLine("Log gravado com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao gravar log: {ex.Message}");
            }
        }
    }
}
