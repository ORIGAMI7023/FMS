namespace FMS.Server.Models.Dtos
{
    public class MonthlySummaryDto
    {
        public decimal TotalMonthly { get; set; }
        public decimal AverageDaily { get; set; }
        public Dictionary<DateOnly, decimal> DailyMap { get; set; } = new();
    }
}
