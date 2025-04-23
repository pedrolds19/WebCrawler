using Microsoft.Extensions.DependencyInjection;
using Services.Extencoes;
using Services.Interfaces;


namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ServiceCollection();

            builder.AddServices();

            var serviceProvider = builder.BuildServiceProvider();

            var homeService = serviceProvider.GetRequiredService<IHomeService>();

            homeService.Iniciar();

            Console.WriteLine("Aplicação de Console Executada com Sucesso!");
        }
    }
}
