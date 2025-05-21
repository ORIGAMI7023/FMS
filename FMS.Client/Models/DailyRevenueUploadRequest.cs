using System;
using System.Collections.Generic;

namespace FMS.Client.Models
{
    public class DailyRevenueUploadRequest
    {
        public DateTime Date { get; set; }
        public string? Remark { get; set; }
        public List<RevenueItem> Items { get; set; } = new();
    }
}