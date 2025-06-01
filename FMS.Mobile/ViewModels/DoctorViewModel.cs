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
            SelectedMonth = AppState.LastHomeMonth ?? DateTime.Today;

            RecordMonth();          
            _ = LoadSummaryAsync(); 

            WeakReferenceMessenger.Default.Register<MonthChangedMessage>(this, (r, m) =>
            {
                DateTime monthFirst = m.Value;
                if (SelectedMonth.Year != monthFirst.Year || SelectedMonth.Month != monthFirst.Month)
                {
                    SelectedMonth = monthFirst;
                    RecordMonth();
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
                DoctorMonthlySummary? dto = await _api.GetDoctorSummaryAsync(SelectedMonth.Year, SelectedMonth.Month);
                if (dto == null) return;

                BusinessDays = dto.BusinessDays;

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
