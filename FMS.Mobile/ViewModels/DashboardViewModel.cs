
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

        public DashboardViewModel()
        {
            _apiService = new ApiService();
            LoadSummaryCommand = new AsyncRelayCommand(LoadSummaryAsync);
        }

        public IAsyncRelayCommand LoadSummaryCommand { get; }

        private async Task LoadSummaryAsync()
        {
            var now = DateTime.Now;

            try
            {
                var result = await _apiService.GetMonthlySummaryAsync(now.Year, now.Month);

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
                // 网络错误或服务不可达
                Console.WriteLine($"[网络错误] {ex.Message}");
                await Shell.Current.DisplayAlert("错误", "无法连接到服务器，请检查网络或服务是否运行。", "确定");

                TotalMonthly = 0;
                TotalToday = 0;
                AverageDaily = 0;
            }
            catch (Exception ex)
            {
                // 其他未捕获异常
                Console.WriteLine($"[系统错误] {ex.Message}");
                await Shell.Current.DisplayAlert("错误", "加载数据时出现异常。", "确定");
            }
        }

    }
}
