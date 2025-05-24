
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
    private ObservableCollection<MonthlyAmountStatistics> amountRecords = new();

    [ObservableProperty]
    private ObservableCollection<MonthlyVisitCountStatistics> visitCountRecords = new();

    [ObservableProperty]
    private DateTime selectedDate = DateTime.Today;

    [ObservableProperty]
    private Chart dailyChart;

    [ObservableProperty]
    private Chart dailyVisitCountChart;

    public DashboardViewModel()
    {
        // 初始不加载数据
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        try
        {
            var amountData = await _api.GetMonthlyAmountStatisticsAsync(SelectedDate.Year, SelectedDate.Month);
            AmountRecords = new ObservableCollection<MonthlyAmountStatistics>(amountData ?? new List<MonthlyAmountStatistics>());

            var visitCountData = await _api.GetMonthlyVisitCountStatisticsAsync(SelectedDate.Year, SelectedDate.Month);
            VisitCountRecords = new ObservableCollection<MonthlyVisitCountStatistics>(visitCountData ?? new List<MonthlyVisitCountStatistics>());

            BuildAmountChart();
            BuildVisitCountChart();
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("提示", "从服务器获取数据失败，已使用默认空图表。", "确定");

            AmountRecords = new ObservableCollection<MonthlyAmountStatistics>();
            VisitCountRecords = new ObservableCollection<MonthlyVisitCountStatistics>();
            BuildAmountChart(true);
            BuildVisitCountChart(true);
        }
    }

    private void BuildAmountChart(bool isFallback = false)
    {
        if ((AmountRecords == null || AmountRecords.Count == 0) && !isFallback)
        {
            DailyChart = null;
            return;
        }

        var entries = (isFallback ? new List<ChartEntry> {
            new ChartEntry(0) { Label = "无数据", ValueLabel = "0", Color = SKColor.Parse("#cccccc") }
        } : AmountRecords.Select(r => new ChartEntry((float)r.TotalAmount)
        {
            Label = $"{r.Owner}-{r.ItemType}",
            ValueLabel = r.TotalAmount.ToString("F0"),
            Color = SKColor.Parse("#6a5acd")
        }).ToList());

        DailyChart = new BarChart { Entries = entries };
    }

    private void BuildVisitCountChart(bool isFallback = false)
    {
        if ((VisitCountRecords == null || VisitCountRecords.Count == 0) && !isFallback)
        {
            DailyVisitCountChart = null;
            return;
        }

        var entries = (isFallback ? new List<ChartEntry> {
            new ChartEntry(0) { Label = "无数据", ValueLabel = "0", Color = SKColor.Parse("#cccccc") }
        } : VisitCountRecords.Select(r => new ChartEntry((float)r.TotalCount)
        {
            Label = $"{r.Owner}-{r.ItemType}",
            ValueLabel = r.TotalCount.ToString("F0"),
            Color = SKColor.Parse("#ff6347")
        }).ToList());

        DailyVisitCountChart = new BarChart { Entries = entries };
    }
}
