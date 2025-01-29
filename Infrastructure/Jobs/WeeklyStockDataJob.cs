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
    public class WeeklyStockDataJob : IJob
    {
        private readonly IElasticSearchService _elasticSearchService;

        public WeeklyStockDataJob(IElasticSearchService elasticSearchService)
        {
            _elasticSearchService = elasticSearchService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            // دریافت لیست نمادها
            var symbols = await _elasticSearchService.GetAllSymbolsAsync();

            // تاریخ امروز
            var today = DateTime.UtcNow;

            // محاسبه تاریخ شروع (شنبه) و پایان (چهارشنبه) هفته
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek); // شنبه
            var endOfWeek = startOfWeek.AddDays(3); // چهارشنبه

            foreach (var symbol in symbols)
            {
                var dailyData = await _elasticSearchService.GetDailyStockDataAsync(symbol, startOfWeek, endOfWeek);

                if (dailyData.Any())
                {
                    var openingPrice = dailyData.First().ClosingPrice; // قیمت شروع هفته
                    var closingPrice = dailyData.Last().ClosingPrice; // قیمت پایان هفته

                    var weeklyData = new WeeklyStockData
                    {
                        Symbol = symbol,
                        OpeningPrice = openingPrice,
                        ClosingPrice = closingPrice,
                        StartDate = startOfWeek,
                        EndDate = endOfWeek,
                        RecordDate = DateTime.UtcNow
                    };

                    await _elasticSearchService.IndexWeeklyStockDataAsync(weeklyData);
                }
            }
        }
    }
}