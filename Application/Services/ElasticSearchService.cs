using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces;
using Domain.Stock;
using Nest;

namespace Application.Services
{
    public class ElasticSearchService : IElasticSearchService
    {
        private readonly IElasticClient _elasticClient;
        public ElasticSearchService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task CreateStockDataIndexAsync()
        {
            // بررسی وجود ایندکس
            var indexExistsResponse = await _elasticClient.Indices.ExistsAsync("stockdata");
            if (!indexExistsResponse.Exists)
            {
                // اگر ایندکس وجود ندارد، آن را ایجاد کنید
                var createIndexResponse = await _elasticClient.Indices.CreateAsync("stockdata", c => c
                  .Settings(s => s
                      .NumberOfShards(1)
                      .NumberOfReplicas(0)
                  )
                  .Mappings(m => m
                      .Map<StockData>(p => p
                          .Properties(ps => ps
                              .Keyword(k => k
                                  .Name(n => n.Symbol)
                              )
                              .Number(f => f
                                  .Name(n => n.ClosingPrice)
                              )
                              .Number(f => f
                                  .Name(n => n.LastTradePrice)
                              )
                              .Date(d => d
                                  .Name(n => n.Date)
                              )
                          )
                      )
                  )
              );
                // نمونه اول
                var stockData1 = new StockData
                {
                    Symbol = "AAPL",
                    ClosingPrice = 150,
                    Date = DateTime.UtcNow,
                    LastTradePrice = 151
                };

                // نمونه دوم
                var stockData2 = new StockData
                {
                    Symbol = "GOOGL",
                    ClosingPrice = 2800,
                    Date = DateTime.UtcNow,
                    LastTradePrice = 2810
                };

                // ایندکس کردن داده‌ها
                var indexResponse1 = await _elasticClient.IndexDocumentAsync(stockData1);
                var indexResponse2 = await _elasticClient.IndexDocumentAsync(stockData2);
            }
        }

        public async Task<List<string>> GetAllSymbolsAsync()
        {
            await CreateStockDataIndexAsync();

            var searchResponse = await _elasticClient.SearchAsync<StockData>(s => s
                    .Index("stockdata")
                    .Size(0) // فقط به متد aggregation نیاز داریم
                    .Aggregations(a => a
                        .Terms("unique_symbols", t => t
                            .Field(f => f.Symbol)
                            .Size(1000) // تعداد نمادها را به اندازه کافی بزرگ انتخاب کنید
                        )
                    )
                );

           var symbols = searchResponse.Aggregations?.Terms("unique_symbols")?.Buckets
            .Select(b => b.Key)
            .ToList() ?? new List<string>();

            return symbols;
        }

        public async Task<List<StockData>> GetAllStockDataAsync()
        {
            var searchResponse = await _elasticClient.SearchAsync<StockData>(s => s
                .Index("stockdata") // نام ایندکس شما
                .Size(1000) // تعداد رکوردهایی که می‌خواهید برگردانید (می‌توانید این مقدار را تغییر دهید)
                .Query(q => q
                    .MatchAll() // بازیابی تمام داده‌ها
                )
            );

            if (!searchResponse.IsValid)
            {
                // مدیریت خطا
                throw new Exception($"Error retrieving data: {searchResponse.OriginalException.Message}");
            }

            return searchResponse.Documents.ToList(); // برگرداندن لیست داده‌ها
        }

        public async Task<List<StockData>> GetDailyStockDataAsync(string symbol, DateTime startDate, DateTime endDate)
        {
            var searchResponse = await _elasticClient.SearchAsync<StockData>(s => s
                .Index("stockdata")
                .Query(q => q
                    .Bool(b => b
                        .Must(m => m
                            .Match(mq => mq
                                .Field(f => f.Symbol)
                                .Query(symbol)),
                            m => m
                                .DateRange(dr => dr
                                    .Field(f => f.Date)
                                    .GreaterThanOrEquals(startDate)
                                    .LessThanOrEquals(endDate)
                                )
                        )
                    )
                )
            );

            return searchResponse.Documents.ToList();
        }

        public async Task IndexMonthlyStockDataAsync(MonthlyStockData monthlyData)
        {
            var response = await _elasticClient.IndexDocumentAsync(monthlyData);
            if (!response.IsValid)
            {
                throw new Exception(response.DebugInformation);
            }
        }

        public async Task IndexStockDataAsync(StockData stockData)
        {
            var response = await _elasticClient.IndexDocumentAsync(stockData);
            if (!response.IsValid)
            {
                throw new Exception(response.DebugInformation);
            }
        }

        public async Task IndexWeeklyStockDataAsync(WeeklyStockData weeklyData)
        {
            var response = await _elasticClient.IndexDocumentAsync(weeklyData);
            if (!response.IsValid)
            {
                throw new Exception(response.DebugInformation);
            }
        }

    }
}
