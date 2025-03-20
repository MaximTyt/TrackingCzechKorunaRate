using Data.Repositories.Abstract;
using Data.Repositories.Implementation;
using Entities.Entity;
using Entities.Entity.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public static class DataModule
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection serviceCollection, string connectionString)
        {
            serviceCollection.AddDbContextFactory<DatabaseContext>(x =>
            {
                x.UseSqlite(connectionString);
            });
            serviceCollection.AddTransient<IRepository<RateEntity>, RateRepository>();
            //serviceCollection.AddScoped<IRepository<RateEntity>, RateRepository>();
            return serviceCollection;
        }
    }
}
