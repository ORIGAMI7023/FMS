using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FMS.Mobile.Messages;
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

        [ObservableProperty] private DateTime selectedMonth = DateTime.Today;

        [ObservableProperty] private int businessDays;

        public DoctorViewModel()
        {
            // 构造时异步加载（不 await，避免阻塞 UI）
            _ = LoadSummaryAsync();   // 默认当月
            RecordMonth();
                
            WeakReferenceMessenger.Default.Register<MonthChangedMessage>(this, (r, m) =>
            {
                DateTime monthFirst = m.Value;
                if (SelectedMonth.Year != monthFirst.Year || SelectedMonth.Month != monthFirst.Month)
                {
                    SelectedMonth = monthFirst;
                    _ = LoadSummaryAsync();
                }
            });
        }

        [RelayCommand]
        private void PrevMonth()
        {
            SelectedMonth = SelectedMonth.AddMonths(-1);
            _ = LoadSummaryAsync();
            RecordMonth();
        }

        [RelayCommand]
        private void NextMonth()
        {
            SelectedMonth = SelectedMonth.AddMonths(1);
            _ = LoadSummaryAsync();
            RecordMonth();
        }

        [ObservableProperty]
        private ObservableCollection<DoctorMonthlySummary.DoctorRow> doctors
            = new ObservableCollection<DoctorMonthlySummary.DoctorRow>();

        // 调试用：总营收与总人次（界面暂不展示）
        [ObservableProperty] private decimal totalMonthlyRevenue;
        [ObservableProperty] private int totalMonthlyVisits;

        private void RecordMonth()
        {
            AppState.LastDoctorMonth = new DateTime(SelectedMonth.Year, SelectedMonth.Month, 1);
        }
        /// <summary>
        /// 调接口并填充属性
        /// </summary>
        private async Task LoadSummaryAsync()
        {
            try
            {
                int y = SelectedMonth.Year;
                int m = SelectedMonth.Month;

                DoctorMonthlySummary? dto = await _api.GetDoctorSummaryAsync(y, m);
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
                System.Diagnostics.Debug.WriteLine($"[DoctorVM] 读取失败: {ex.Message}");
            }
        }
    }
}
