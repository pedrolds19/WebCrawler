using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Extencoes;
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
            builder.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


            builder.AddServices();
            builder.AddSingleton<IConfiguration>(configuration);
            var serviceProvider = builder.BuildServiceProvider();

            var homeService = serviceProvider.GetRequiredService<IHomeService>();

            homeService.Iniciar();

            Console.WriteLine("Aplicação de Console Executada com Sucesso!");
        }
    }
}
