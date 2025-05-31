
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FMS.Mobile.Models;
using FMS.Mobile.Services;
using System;
using System.Threading.Tasks;
using FMS.Mobile.Messages;


namespace FMS.Mobile.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly ApiService _apiService;//接口服务实例

        [ObservableProperty] private decimal totalMonthly;

        [ObservableProperty] private decimal totalToday;

        [ObservableProperty] private decimal averageDaily;

        [ObservableProperty] private DateTime selectedDate = DateTime.Today;//初始化时，默认选中今天

        [ObservableProperty] private Dictionary<DateOnly, decimal> dailyMap = new();

        private DateOnly currentMonth;
        private DateTime _loadingMonth = DateTime.MinValue;

        public DashboardViewModel()
        {
            _apiService = new ApiService();
            LoadSummaryAsync();//初始化获取数据
        }

        /// <summary>
        /// 钩子方法，当 SelectedDate 属性改变时调用
        /// </summary>
        /// <param name="value"></param>
        partial void OnSelectedDateChanged(DateTime value)
        {
            if (value.Month != currentMonth.Month || value.Year != currentMonth.Year)
            {
                LoadSummaryAsync(); 
            }
            else
            {
                ChangeSelectedDate();
            }
        }


        /// <summary>
        /// 在同一个月内切换日期时调用，从本地缓存中获取数据
        /// </summary>
        private void ChangeSelectedDate()
        {
            DateOnly date = DateOnly.FromDateTime(SelectedDate);//获取当前选中的日期
            TotalToday = DailyMap.ContainsKey(date) ? DailyMap[date] : 0;//如果今天有数据则显示，否则为0
        }

        /// <summary>
        /// 切换月份时调用，重新加载整月数据
        /// </summary>
        /// <returns></returns>
        private async Task LoadSummaryAsync()
        {
            DateOnly date = DateOnly.FromDateTime(SelectedDate);
            DateTime thisMonth = new(date.Year, date.Month, 1);
            if (_loadingMonth == thisMonth) return;
            _loadingMonth = thisMonth;

            try
            {
                MonthlySummary? result = await _apiService.GetMonthlySummaryAsync(date);
                if (result == null)
                {
                    await Shell.Current.DisplayAlert("提示", "未找到本月数据。", "确定");
                    return;
                }

                TotalMonthly = result.TotalMonthly;
                AverageDaily = result.AverageDaily;
                DailyMap = result.DailyMap;
                currentMonth = date;
                AppState.LastHomeMonth = new DateTime(date.Year, date.Month, 1);

                WeakReferenceMessenger.Default.Send(new MonthChangedMessage(thisMonth));

                TotalToday = DailyMap.ContainsKey(date) ? DailyMap[date] : 0;
                if (TotalToday == 0 && date == DateOnly.FromDateTime(DateTime.Now))
                    await Shell.Current.DisplayAlert("提示", "今天还没有收入记录，请稍后查看。", "确定");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"[网络错误] {ex.Message}");
                await Shell.Current.DisplayAlert("错误", $"无法连接到服务器。{ex.Message}", "确定");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[系统错误] {ex.Message}");
                await Shell.Current.DisplayAlert("错误", "加载数据时出现异常。", "确定");
            }
        }
    }
}
