using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class Rate
    {
        public DateOnly Date { get; set; }
        public string Currency { get; set; }
        public decimal RateValue { get; set; }
    }
}
