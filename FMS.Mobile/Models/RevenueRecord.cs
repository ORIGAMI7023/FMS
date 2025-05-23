namespace FMS.Mobile.Models;

public class RevenueRecord
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string Doctor { get; set; }
    public string Department { get; set; }
    public string ItemType { get; set; }
    public decimal Amount { get; set; }
    public string Source { get; set; }
}