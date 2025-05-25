namespace FMS.Server.Models.Dtos
{
    public class MonthlySummaryDto
    {
        public decimal TotalMonthly { get; set; }
        public decimal TotalToday { get; set; }
        public decimal AverageDaily { get; set; }
    }
}
