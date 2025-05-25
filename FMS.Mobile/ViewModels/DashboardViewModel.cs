
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FMS.Mobile.Models;
using FMS.Mobile.Services;
using System;
using System.Threading.Tasks;

namespace FMS.Mobile.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private decimal totalMonthly;

        [ObservableProperty]
        private decimal totalToday;

        [ObservableProperty]
        private decimal averageDaily;

        [ObservableProperty]
        private DateTime selectedDate = DateTime.Today;

        public DashboardViewModel()
        {
            _apiService = new ApiService();
            LoadSummaryCommand = new AsyncRelayCommand(LoadSummaryAsync);
        }

        public IAsyncRelayCommand LoadSummaryCommand { get; }

        partial void OnSelectedDateChanged(DateTime value)
        {
            LoadSummaryCommand.Execute(null);
        }

        private async Task LoadSummaryAsync()
        {
            DateOnly date = DateOnly.FromDateTime(SelectedDate);
            try
            {
                var result = await _apiService.GetMonthlySummaryAsync(date);
                if (result != null)
                {
                    TotalMonthly = result.TotalMonthly;
                    TotalToday = result.TotalToday;
                    AverageDaily = result.AverageDaily;
                }
                else
                {
                    TotalMonthly = 0;
                    TotalToday = 0;
                    AverageDaily = 0;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"[网络错误] {ex.Message}");
                await Shell.Current.DisplayAlert("错误", "无法连接到服务器，请检查网络或服务是否运行。", "确定");
                TotalMonthly = 0;
                TotalToday = 0;
                AverageDaily = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[系统错误] {ex.Message}");
                await Shell.Current.DisplayAlert("错误", "加载数据时出现异常。", "确定");
            }
        }
    }
}
