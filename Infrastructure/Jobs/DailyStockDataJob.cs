using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Domain.Interfaces;
using Domain.Stock;
using Quartz;

namespace Infrastructure.Jobs
{
    public class DailyStockDataJob : IJob
    {
        private readonly IElasticSearchService _elasticSearchService;
        private readonly IStockService _stockService;

        public DailyStockDataJob(IElasticSearchService elasticSearchService, IStockService stockService)
        {
            _elasticSearchService = elasticSearchService;
            _stockService = stockService;
        }

        public async Task Execute(IJobExecutionContext context)
        {

            // دریافت لیست نمادها
            var stockPriceData = await _stockService.GetAllClosingDayAsync();

            foreach (var stock in stockPriceData)
            {
                var stockData = new StockData
                {
                    Symbol = stock.Symbol,
                    ClosingPrice = stock.ClosingPrice,
                    Date = stock.DateOfEvent, // تاریخ فعلی
                    LastTradePrice = stock.LastTradePrice
                };

                await _elasticSearchService.IndexStockDataAsync(stockData);
            }

          //var xx=await  _elasticSearchService.GetAllStockDataAsync();


        }
    }
}
