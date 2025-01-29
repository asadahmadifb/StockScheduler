using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Stock
{
    public class StockData
    {
        public string Symbol { get; set; }
        public double ClosingPrice { get; set; }
        public DateTime Date { get; set; }
        public double LastTradePrice { get; set; }
    }
}
