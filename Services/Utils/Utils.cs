using System.Collections.Concurrent;

namespace Services.Utils
{
    public static class Utils
    {
        public static ConcurrentDictionary<string, decimal> ParseData(Task<string> data)
        {
            var lines = data.Result.Split('\n')[2..^1];
            var rates = new ConcurrentDictionary<string, decimal>();
            Parallel.ForEach(lines, line =>
            {
                var part = line.Split('|');
                int amount = Convert.ToInt32(part[2]);
                string currency = part[3];
                decimal rate = Convert.ToDecimal(part[4].Replace('.', ',')) / amount;
                rates.TryAdd(currency, rate);
            });
            return rates;
        }
    }
}
