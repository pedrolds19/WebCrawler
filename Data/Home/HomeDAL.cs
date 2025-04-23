using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCrawler.Data;  // Certifique-se de importar seu DbContext

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
                // Adiciona o log na tabela CrawlerLogs
                await _context.CrawlerLogs.AddAsync(log);
                await _context.SaveChangesAsync(); // Salva no banco de dados

                Console.WriteLine("Log gravado com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao gravar log: {ex.Message}");
            }
        }
    }
}
