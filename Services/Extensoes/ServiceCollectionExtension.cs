using Data.Home;
using Microsoft.Extensions.DependencyInjection;
using Services.Implementacoes;
using Services.Interfaces;


namespace Services.Extencoes
{
    public static class ServiceCollectionExtension
    {

        public static IServiceCollection AddServices(this IServiceCollection services)
        {

            services.AddScoped<IHomeService, HomeService>();
            services.AddScoped<HomeDAL>();
            return services;
        }

    }
}
