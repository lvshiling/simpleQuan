using System.Text.Json;

namespace SimpleQuanBlazor.Data
{
    public class StockService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public StockService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<StockInfo>> GetTop100ByTurnoverAsync()
        {
            var client = _httpClientFactory.CreateClient();
            
            // 东方财富实时数据接口 (不支持历史查询)
            // fields=f12,f14,f2,f3,f6,f5,f62,f20 (总市值), f21 (流通市值)
            string url = "https://push2.eastmoney.com/api/qt/clist/get?pn=1&pz=100&po=1&np=1&ut=bd1d9ddb04089700cf9c27f6f7426281&fltt=2&invt=2&fid=f6&fs=m:0+t:6,m:0+t:80,m:1+t:2,m:1+t:23,m:0+t:81+s:2048&fields=f12,f14,f2,f3,f6,f5,f62,f20,f21";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return new List<StockInfo>();

            var jsonString = await response.Content.ReadAsStringAsync();
            var root = JsonDocument.Parse(jsonString).RootElement;
            if (!root.TryGetProperty("data", out var dataElement) || !dataElement.TryGetProperty("diff", out var diffElement))
            {
                return new List<StockInfo>();
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
                    Volume = GetDouble(item, "f5"),
                    NetInflow = GetDouble(item, "f62"),
                    TotalMarketCap = GetDouble(item, "f20"),
                    FloatMarketCap = GetDouble(item, "f21")
                });
            }
            return stocks;
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
