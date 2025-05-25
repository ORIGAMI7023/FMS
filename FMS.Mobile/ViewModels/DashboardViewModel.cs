
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
                // ����������񲻿ɴ�
                Console.WriteLine($"[�������] {ex.Message}");
                await Shell.Current.DisplayAlert("����", "�޷����ӵ����������������������Ƿ����С�", "ȷ��");

                TotalMonthly = 0;
                TotalToday = 0;
                AverageDaily = 0;
            }
            catch (Exception ex)
            {
                // ����δ�����쳣
                Console.WriteLine($"[ϵͳ����] {ex.Message}");
                await Shell.Current.DisplayAlert("����", "��������ʱ�����쳣��", "ȷ��");
            }
        }

    }
}
