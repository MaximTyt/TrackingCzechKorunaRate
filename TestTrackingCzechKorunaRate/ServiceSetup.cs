using Data.Repositories.Abstract;
using Data.Repositories.Implementation;
using Entities.Entity;
using Entities.Entity.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.Utils;
using TrackingCzechKorunaRate.Controllers;

namespace TestTrackingCzechKorunaRate
{
    public class ServiceSetup
    {
        public static ServiceProvider AddService(ServiceCollection services)
        {
            // Регистрация in-memory базы данных
            services.AddDbContextFactory<DatabaseContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase");
            });
            //Регистрация конфигурации
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory() + "../../../../../TrackingCzechKorunaRate/Config")
                .AddJsonFile("config.json")
                .Build();
            services.Configure<BaseAddressSetting>(configuration.GetSection("BaseAddress"));

            // Регистрация сервисов
            services.AddTransient<IRepository<RateEntity>, RateRepository>();
            services.AddBusinessLogic();
            services.AddScoped<RateController>();
            services.AddLogging();
            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }
    }
}
