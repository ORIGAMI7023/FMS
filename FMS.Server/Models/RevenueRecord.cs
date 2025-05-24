using System.ComponentModel.DataAnnotations;

namespace FMS.Server.Models;

public class RevenueRecord
{
    [Key]
    public Guid Id { get; set; }  // ����

    public DateTime Date { get; set; }  // ����

    public string Owner { get; set; } = null!;  // ҽ���������

    public string ItemType { get; set; } = null!;  // �ֽ�ҽ�����˴ε�

    public decimal Value { get; set; }  // �����˴�

    public bool IsVisitCount { get; set; }  // �Ƿ�Ϊ�˴���������

    public bool IsExcludedFromSummary { get; set; }  // �Ƿ�ӻ������ų�

    public string Source { get; set; }  // ������Դ���磺Server��Client�ȣ�
}
