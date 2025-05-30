using System.Collections.Generic;

namespace FMS.Mobile.Models
{
    /// <summary>
    /// 当前月份医生营收汇总 DTO（对应服务端 DoctorMonthlySummaryDto）
    /// </summary>
    public class DoctorMonthlySummary
    {
        public int BusinessDays { get; set; }
        public decimal TotalMonthlyRevenue { get; set; }   // 仅调试用
        public int TotalMonthlyVisits { get; set; }        // 仅调试用

        public List<DoctorRow> Doctors { get; set; } = new();

        public class DoctorRow
        {
            public string Owner { get; set; } = string.Empty;
            public decimal TotalRevenue { get; set; }
            public int TotalVisits { get; set; }
        }
    }
}
