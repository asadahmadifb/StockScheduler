using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Stock
{
    public class MonthlyStockData
    {
        public string Symbol { get; set; }
        public double OpeningPrice { get; set; }
        public double ClosingPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime RecordDate { get; set; }
    }
}
