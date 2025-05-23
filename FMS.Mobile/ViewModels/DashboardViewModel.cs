using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FMS.Mobile.Models;
using FMS.Mobile.Services;
using Microcharts;
using SkiaSharp;

namespace FMS.Mobile.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly ApiService _api = new();

    [ObservableProperty]
    private ObservableCollection<ItemTypeStatistics> records = new();

    // 把 DateOnly 改为 DateTime，初始值为 DateTime.Today
    [ObservableProperty]
    private DateTime selectedDate = DateTime.Today;

    [ObservableProperty]
    private Chart dailyChart;

    public DashboardViewModel()
    {
        LoadDataCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        // 用 DateTime，直接传给API
        var data = await _api.GetStatisticsAsync(selectedDate);

        // Debug 输出接口数据数量
        Console.WriteLine("Loaded records count: " + (data?.Count ?? 0));
        if (data != null)
        {
            foreach (var item in data)
                Console.WriteLine($"{item.itemType}: {item.totalAmount}");
        }

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

        if (entries == null || entries.Count == 0)
        {
            DailyChart = null;
            return;
        }

        DailyChart = new BarChart { Entries = entries };
    }
}
