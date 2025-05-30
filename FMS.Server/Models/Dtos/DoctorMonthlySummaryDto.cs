namespace FMS.Server.Models.Dtos
{
    namespace FMS.Server.Models.Dtos
    {
        /// <summary>
        /// 当前月份医生汇总 DTO
        /// </summary>
        public class DoctorMonthlySummaryDto
        {
            public int BusinessDays { get; set; }          // 本月营业天数
            public decimal TotalMonthlyRevenue { get; set; } // 本月总营收（调试用）
            public int TotalMonthlyVisits { get; set; }     // 本月总人次（调试用）

            public List<DoctorRow> Doctors { get; set; } = new();

            public class DoctorRow
            {
                public string Owner { get; set; } = string.Empty;
                public decimal TotalRevenue { get; set; }   // 医生总营收
                public int TotalVisits { get; set; }        // 医生总人次
            }
        }
    }

}
