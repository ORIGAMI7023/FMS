using System.ComponentModel.DataAnnotations;

namespace FMS.Server.Models;

public class RevenueRecord
{
    [Key]
    public Guid Id { get; set; }

    public DateOnly Date { get; set; }

    public string? Doctor { get; set; }

    public string ItemType { get; set; }

    public decimal Amount { get; set; }

    public string Department { get; set; }

    public string Source { get; set; }

    public bool IsVisitCount { get; set; }
}
