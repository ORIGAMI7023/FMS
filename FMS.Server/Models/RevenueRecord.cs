using System.ComponentModel.DataAnnotations;

namespace FMS.Server.Models;

public class RevenueRecord
{
    [Key]
    public Guid Id { get; set; }  // 主键

    public DateTime Date { get; set; }  // 日期

    public string Owner { get; set; } = null!;  // 医生名或科室

    public string ItemType { get; set; } = null!;  // 现金、医保、人次等

    public decimal Value { get; set; }  // 金额或人次

    public bool IsVisitCount { get; set; }  // 是否为人次类型数据

    public bool IsExcludedFromSummary { get; set; }  // 是否从汇总中排除

    public string Source { get; set; }  // 数据来源（如：Server、Client等）
}
