using Microsoft.AspNetCore.Mvc;
using SimpleQuanApp.Models;
using System.Text.Json;

namespace SimpleQuanApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<StockController> _logger;

        public StockController(IHttpClientFactory httpClientFactory, ILogger<StockController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpGet("top100")]
        public async Task<ActionResult<IEnumerable<StockInfo>>> GetTop100ByTurnover()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                // 东方财富接口示例: pz=100 (每页100条), po=1 (由高到低), np=1 (第1页), fltt=2, invt=2, fid=f6 (成交额)
                // fs=m:0+t:6,m:0+t:80,m:1+t:2,m:1+t:23,m:0+t:81+s:2048 (沪深京A股)
                string url = "https://push2.eastmoney.com/api/qt/clist/get?pn=1&pz=100&po=1&np=1&ut=bd1d9ddb04089700cf9c27f6f7426281&fltt=2&invt=2&fid=f6&fs=m:0+t:6,m:0+t:80,m:1+t:2,m:1+t:23,m:0+t:81+s:2048&fields=f12,f14,f2,f3,f6,f5";
                
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Failed to fetch data from source.");
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                var root = JsonDocument.Parse(jsonString).RootElement;
                if (!root.TryGetProperty("data", out var dataElement) || !dataElement.TryGetProperty("diff", out var diffElement))
                {
                    return Ok(new List<StockInfo>());
                }

                var stocks = new List<StockInfo>();
                foreach (var item in diffElement.EnumerateArray())
                {
                    stocks.Add(new StockInfo
                    {
                        Code = item.GetProperty("f12").GetString() ?? "",
                        Name = item.GetProperty("f14").GetString() ?? "",
                        Price = GetDouble(item, "f2"),
                        ChangePercent = GetDouble(item, "f3"),
                        Turnover = GetDouble(item, "f6"),
                        Volume = GetDouble(item, "f5")
                    });
                }

                return Ok(stocks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching stock data");
                return StatusCode(500, "Internal server error");
            }
        }

        private static double GetDouble(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop))
            {
                if (prop.ValueKind == JsonValueKind.Number) return prop.GetDouble();
                if (prop.ValueKind == JsonValueKind.String && double.TryParse(prop.GetString(), out var val)) return val;
            }
            return 0;
        }
    }
}
