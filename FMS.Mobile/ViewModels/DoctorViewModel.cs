using CommunityToolkit.Mvvm.ComponentModel;
using FMS.Mobile.Models;
using FMS.Mobile.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FMS.Mobile.ViewModels
{
    /// <summary>
    /// Doctor 页面 VM：负责加载当前月份医生营收汇总
    /// </summary>
    public partial class DoctorViewModel : ObservableObject
    {
        private readonly ApiService _api = new ApiService();

        [ObservableProperty]
        private int businessDays;

        [ObservableProperty]
        private ObservableCollection<DoctorMonthlySummary.DoctorRow> doctors
            = new ObservableCollection<DoctorMonthlySummary.DoctorRow>();

        // 调试用：总营收与总人次（界面暂不展示）
        [ObservableProperty] private decimal totalMonthlyRevenue;
        [ObservableProperty] private int totalMonthlyVisits;

        public DoctorViewModel()
        {
            // 构造时异步加载（不 await，避免阻塞 UI）
            _ = LoadSummaryAsync();
        }

        /// <summary>
        /// 调接口并填充属性
        /// </summary>
        private async Task LoadSummaryAsync()
        {
            try
            {
                DoctorMonthlySummary? dto = await _api.GetDoctorSummaryCurrentMonthAsync();
                if (dto == null) return;

                BusinessDays = dto.BusinessDays;
                TotalMonthlyRevenue = dto.TotalMonthlyRevenue;
                TotalMonthlyVisits = dto.TotalMonthlyVisits;

                Doctors.Clear();
                foreach (DoctorMonthlySummary.DoctorRow row in dto.Doctors)
                    Doctors.Add(row);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DoctorViewModel] 读取失败: {ex.Message}");
                // 生产环境可弹 Toast 或对话框
            }
        }
    }
}
