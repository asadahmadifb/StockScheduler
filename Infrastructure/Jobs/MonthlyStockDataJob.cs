using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces;
using Domain.Stock;
using Quartz;

namespace Infrastructure.Jobs
{
    public class MonthlyStockDataJob : IJob
    {
        private readonly IElasticSearchService _elasticSearchService;

        public MonthlyStockDataJob(IElasticSearchService elasticSearchService)
        {
            _elasticSearchService = elasticSearchService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            // دریافت لیست نمادها
            var symbols = await _elasticSearchService.GetAllSymbolsAsync();
            var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var endOfMonth = DateTime.UtcNow.Date;

            foreach (var symbol in symbols)
            {
                var dailyData = await _elasticSearchService.GetDailyStockDataAsync(symbol, startOfMonth, endOfMonth);

                if (dailyData.Any())
                {
                    var openingPrice = dailyData.First().ClosingPrice; // قیمت شروع ماه
                    var closingPrice = dailyData.Last().ClosingPrice; // قیمت پایان ماه

                    var monthlyData = new MonthlyStockData
                    {
                        Symbol = symbol,
                        OpeningPrice = openingPrice,
                        ClosingPrice = closingPrice,
                        StartDate = startOfMonth,
                        EndDate = endOfMonth,
                        RecordDate = DateTime.UtcNow
                    };

                    await _elasticSearchService.IndexMonthlyStockDataAsync(monthlyData);
                }
            }
        }
    }
}
