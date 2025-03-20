using Microsoft.Extensions.DependencyInjection;
using Services.Service.Abstract;
using Services.Service.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public static class ServiceModule
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IRateService, RateService>();            
            return serviceCollection;
        }
    }
}
