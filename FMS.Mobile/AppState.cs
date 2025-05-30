using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Mobile
{
    public static class AppState
    {
        /// <summary>
        /// 医生页最后一次选中的月份（1 号日期）；null 表示未设置
        /// </summary>
        public static DateTime? LastDoctorMonth { get; set; }
    }
}
