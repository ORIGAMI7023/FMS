namespace FMS.Mobile.Models
{
    public class MonthlySummary
    {
        public decimal TotalMonthly { get; set; }
        public decimal AverageDaily { get; set; }
        public Dictionary<DateOnly, decimal> DailyMap { get; set; } = new();
    }
}
