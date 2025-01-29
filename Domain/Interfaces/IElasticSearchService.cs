using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Stock;

namespace Domain.Interfaces
{
    public interface IElasticSearchService
    {
        Task IndexStockDataAsync(StockData stockData);
        Task IndexWeeklyStockDataAsync(WeeklyStockData weeklyData);
        Task IndexMonthlyStockDataAsync(MonthlyStockData monthlyData);
        Task<List<StockData>> GetDailyStockDataAsync(string symbol, DateTime startDate, DateTime endDate);
        Task<List<string>> GetAllSymbolsAsync();
        Task<List<StockData>> GetAllStockDataAsync();
        Task CreateStockDataIndexAsync(); // متد جدید برای ایجاد ایندکس

    }
}
