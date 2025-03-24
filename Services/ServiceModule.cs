using Microsoft.Extensions.DependencyInjection;
using Services.Service.Abstract;
using Services.Service.Implementation;
using Services.Utils;

namespace Services
{
    public static class ServiceModule
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection serviceCollection)
        {            
            serviceCollection.AddTransient<IRateService, RateService>();
            serviceCollection.AddTransient<ICNBRequest, CNBRequest>();
            return serviceCollection;
        }
    }
}
