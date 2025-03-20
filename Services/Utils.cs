using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public static class Utils
    {
        public static Dictionary<string, decimal> ParseData(Task<string> data)
        {
            var lines = data.Result.Split('\n')[2..^1];
            var rates = new Dictionary<string, decimal>();
            foreach(var line in lines)
            {
                var part = line.Split('|');
                int amount = Convert.ToInt32(part[2]);
                string currency = part[3];
                decimal rate = Convert.ToDecimal(part[4].Replace('.',',')) / amount;
                rates.Add(currency, rate);
            }
            return rates;
        }
    }
}
