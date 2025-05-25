using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Mobile.Models
{
    public class MonthlySummary
    {
        public decimal TotalMonthly { get; set; }
        public decimal TotalToday { get; set; }
        public decimal AverageDaily { get; set; }
    }
}
