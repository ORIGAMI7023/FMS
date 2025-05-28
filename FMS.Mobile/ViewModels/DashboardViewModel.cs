
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
        private readonly ApiService _apiService;//接口服务实例

        [ObservableProperty]
        private decimal totalMonthly;

        [ObservableProperty]
        private decimal totalToday;

        [ObservableProperty]
        private decimal averageDaily;

        [ObservableProperty]
        private DateTime selectedDate = DateTime.Today;//初始化时，默认选中今天

        [ObservableProperty]
        private Dictionary<DateOnly, decimal> dailyMap = new();

        private DateOnly currentMonth;

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
            if (value.Month != currentMonth.Month || value.Year != currentMonth.Year)//当切换月份时，重新加载月数据
            {
                LoadSummaryAsync();
            }
            else//未切换月份，从本地缓存中获取数据
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
            DateOnly date = DateOnly.FromDateTime(SelectedDate);//获取当前选中的日期

            try
            {
                MonthlySummary? result = await _apiService.GetMonthlySummaryAsync(date);//调用接口获取指定日期的月度收入摘要
                if (result == null)
                {
                    await Shell.Current.DisplayAlert("提示", "未找到本月数据。", "确定");
                    return;
                }

                TotalMonthly = result.TotalMonthly;
                AverageDaily = result.AverageDaily;
                DailyMap = result.DailyMap;
                currentMonth = date;//更新当前月份

                TotalToday = DailyMap.ContainsKey(date) ? DailyMap[date] : 0;//如果今天有数据则显示，否则为0
                if (TotalToday == 0 && date == DateOnly.FromDateTime(DateTime.Now))
                    await Shell.Current.DisplayAlert("提示", "今天还没有收入记录，请稍后查看。", "确定");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"[网络错误] {ex.Message}");
                await Shell.Current.DisplayAlert("错误", $"无法连接到服务器。{ex.Message}", "确定");
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
