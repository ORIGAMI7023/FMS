using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FMS.Mobile.Models;
using FMS.Mobile.Services;
using Microcharts;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace FMS.Mobile.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly ApiService _api = new();

    [ObservableProperty]
    private ObservableCollection<ItemTypeStatistics> records = new();

    [ObservableProperty]
    private DateOnly selectedDate = DateOnly.FromDateTime(DateTime.Today);

    [ObservableProperty]
    private Chart dailyChart;

    public DashboardViewModel()
    {
        LoadDataCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        var data = await _api.GetStatisticsAsync(selectedDate.ToDateTime(TimeOnly.MinValue));
        Records = new ObservableCollection<ItemTypeStatistics>(data ?? new List<ItemTypeStatistics>());
        BuildChart();
    }

    private void BuildChart()
    {
        var entries = Records.Select(r => new ChartEntry((float)r.totalAmount)
        {
            Label = r.itemType,
            ValueLabel = r.totalAmount.ToString("F0"),
            Color = SKColor.Parse("#6a5acd")
        }).ToList();

        // 增加判空处理
        if (entries == null || entries.Count == 0)
        {
            DailyChart = null;
            return;
        }

        DailyChart = new BarChart { Entries = entries };
    }
}