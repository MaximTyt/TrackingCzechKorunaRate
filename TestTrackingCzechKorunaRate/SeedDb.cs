using Entities.Entity.EF;
using Entities.Entity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTrackingCzechKorunaRate
{
    public class SeedData
    {
        public static void SeedDb(ServiceProvider serviceProvider)
        {
            // Заполнение базы данных тестовыми данными
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                dbContext.Database.EnsureDeleted();

                dbContext.Rates.AddRange(new List<RateEntity>
                {
                    new RateEntity { Currency = "USD", RateValue = 22.968m, Date = DateOnly.Parse("17.03.2025") },
                    new RateEntity { Currency = "EUR", RateValue = 25.040m, Date = DateOnly.Parse("17.03.2025") },
                    new RateEntity { Currency = "USD", RateValue = 22.935m, Date = DateOnly.Parse("18.03.2025") },
                    new RateEntity { Currency = "EUR", RateValue = 25.035m, Date = DateOnly.Parse("18.03.2025") },
                    new RateEntity { Currency = "USD", RateValue = 22.938m, Date = DateOnly.Parse("19.03.2025") },
                    new RateEntity { Currency = "EUR", RateValue = 25.000m, Date = DateOnly.Parse("19.03.2025") }
                });
                //(25,04 + 25,035 + 25)
                //(22,968 + 22,935 + 22,938)/3

                dbContext.SaveChanges();
            }
        }
    }
}
