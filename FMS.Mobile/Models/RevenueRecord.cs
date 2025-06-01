using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Mobile.Models
{
    public class RevenueRecord
    {
        public DateOnly Date { get; set; }
        public string ItemType { get; set; } = "";
        public decimal Value { get; set; }
        public bool IsVisitCount { get; set; }
    }

}
