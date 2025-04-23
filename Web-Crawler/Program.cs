using Data.Home;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Extencoes;
using Services.Implementacoes;
using Services.Interfaces;
using WebCrawler.Data;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            var builder = new ServiceCollection();

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.AddScoped<IHomeService, HomeService>();

            builder.AddScoped<HomeDAL>();

            builder.AddSingleton<IConfiguration>(configuration);

            var serviceProvider = builder.BuildServiceProvider();

            var homeService = serviceProvider.GetRequiredService<IHomeService>();

            var log = await homeService.Iniciar();

            var homeDAL = serviceProvider.GetRequiredService<HomeDAL>();
            await homeDAL.GravarLog(log);

            Console.WriteLine("Aplicação de Console Executada com Sucesso!");
        }
    }
}
