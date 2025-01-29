using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class StockPriceResponse
    {
        public string Symbol { get; set; }
        public double ClosingPrice { get; set; }
        public double LastTradePrice { get; set; }
        public DateTime DateOfEvent { get; set; }
    }
}