namespace SimpleQuanBlazor.Data
{
    public class StockInfo
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public double ChangePercent { get; set; }
        public double Turnover { get; set; } // 成交额
        public double Volume { get; set; }   // 成交量
        public double NetInflow { get; set; } // 今日主力净流入
    }
}
