using Data.Repositories.Abstract;
using Data.Repositories.Implementation;
using Entities.Entity.EF;
using Entities.Entity;
using Microsoft.Extensions.DependencyInjection;
using Services.Service.Abstract;
using Services.Service.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackingCzechKorunaRate.Controllers;
using Microsoft.EntityFrameworkCore;

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
            // Регистрация сервисов
            services.AddTransient<IRepository<RateEntity>, RateRepository>();
            services.AddTransient<IRateService, RateService>();
            services.AddScoped<RateController>();
            services.AddLogging();
            var serviceProvider = services.BuildServiceProvider();            
            return serviceProvider;
        }
    }
}
