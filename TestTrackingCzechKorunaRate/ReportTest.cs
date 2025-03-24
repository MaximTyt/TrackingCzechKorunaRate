using Microsoft.Extensions.DependencyInjection;
using TrackingCzechKorunaRate.Controllers;

namespace TestTrackingCzechKorunaRate
{
    [TestClass]
    public sealed class ReportTest
    {   

        //Обычная ситуация (все данные присутствуют)
        [TestMethod]
        public async Task GetReportTest()
        {
            var services = new ServiceCollection();
            var serviceProvider = ServiceSetup.AddService(services);
            SeedData.SeedDb(serviceProvider);

            var controller = serviceProvider.GetRequiredService<RateController>();

            // Act
            var result = await controller.GetReportAsync("17.03.2025", "19.03.2025", ["USD", "EUR" ]);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() == 2);

            var usd_report = result.Where(x => x.Currency == "USD").First();
            Assert.IsTrue(usd_report.MinRate == 22.935m);
            Assert.IsTrue(usd_report.MaxRate == 22.968m);
            Assert.IsTrue(usd_report.AvgRate == 22.947m);

            var eur_report = result.Where(x => x.Currency == "EUR").First();
            Assert.IsTrue(eur_report.MinRate == 25m);
            Assert.IsTrue(eur_report.MaxRate == 25.04m);
            Assert.IsTrue(eur_report.AvgRate == 25.025m);
        }
        //Отстутсвует данные по одной из валют
        [TestMethod]
        public async Task GetReporWithoutRubCurrencyTest()
        {
            var services = new ServiceCollection();
            var serviceProvider = ServiceSetup.AddService(services);
            SeedData.SeedDb(serviceProvider);

            var controller = serviceProvider.GetRequiredService<RateController>();

            // Act
            var result = await controller.GetReportAsync("17.03.2025", "19.03.2025", ["USD", "EUR", "RUB"]);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() == 2);
            var rub_report = result.Where(x => x.Currency == "RUB").FirstOrDefault();
            Assert.IsNull(rub_report);
        }
        //Полное отсутсвие данных за период
        [TestMethod]
        public async Task GetReporWithoutResultTest()
        {
            var services = new ServiceCollection();
            var serviceProvider = ServiceSetup.AddService(services);
            SeedData.SeedDb(serviceProvider);

            var controller = serviceProvider.GetRequiredService<RateController>();

            // Act
            var result = await controller.GetReportAsync("17.03.2026", "19.03.2026", ["USD", "EUR"]);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() == 0);
        }
    }
}
