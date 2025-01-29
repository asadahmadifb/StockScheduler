using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces;
using Domain.Models;

namespace Application.Services
{
    public class StockService : IStockService
    {
        private readonly HttpClient _httpClient;

        public StockService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<StockPriceResponse>> GetAllClosingDayAsync()
        {
            //var response = await _httpClient.GetAsync("https://api.example.com/stocks/today"); // URL واقعی API
            //if (response.IsSuccessStatusCode)
            //{
            //    return await response.Content.ReadFromJsonAsync<List<StockPriceResponse>>();
            //}
            //return new List<StockPriceResponse>();
            var stockPriceResponses = new List<StockPriceResponse>
        {
            new StockPriceResponse
            {
                Symbol = "AAPL",
                ClosingPrice = 150.25,
                LastTradePrice = 151.00,
                DateOfEvent = new DateTime(2025, 1, 28)
            },
            new StockPriceResponse
            {
                Symbol = "GOOGL",
                ClosingPrice = 2800.50,
                LastTradePrice = 2810.75,
                DateOfEvent = new DateTime(2025, 1, 28)
            },
            new StockPriceResponse
            {
                Symbol = "MSFT",
                ClosingPrice = 300.10,
                LastTradePrice = 299.90,
                DateOfEvent = new DateTime(2025, 1, 28)
            },
            new StockPriceResponse
            {
                Symbol = "AMZN",
                ClosingPrice = 3500.00,
                LastTradePrice = 3495.50,
                DateOfEvent = new DateTime(2025, 1, 28)
            },
            new StockPriceResponse
            {
                Symbol = "TSLA",
                ClosingPrice = 700.25,
                LastTradePrice = 705.00,
                DateOfEvent = new DateTime(2025, 1, 28)
            }
        };
            return stockPriceResponses;
        }
    }
}
