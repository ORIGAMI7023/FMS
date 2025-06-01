using CommunityToolkit.Mvvm.ComponentModel;
using FMS.Mobile.Services;
using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Mobile.ViewModels
{
    public partial class DoctorDetailViewModel : ObservableObject
    {
        private readonly ApiService _apiService = new();

        [ObservableProperty] private string owner = "";
        [ObservableProperty] private DateTime selectedMonth = DateTime.Today;
        [ObservableProperty] private Dictionary<DateOnly, decimal> revenueMap = new();
        [ObservableProperty] private Dictionary<DateOnly, int> visitMap = new();
        [ObservableProperty] private Chart chartModel;   // ← 名称改为 ChartModel


        public DoctorDetailViewModel(string owner, DateTime month)
        {
            Owner = owner;
            SelectedMonth = month;
            LoadDetailAsync();


        }

        private async Task LoadDetailAsync()
        {
            try
            {
                // 1. 生成两条 ChartEntry 列表
                var revEntries = RevenueMap.OrderBy(k => k.Key)
                    .Select(k => new ChartEntry((float)k.Value)
                    {
                        ValueLabel = $"{k.Value:F0}",
                        Color = SKColors.SteelBlue,
                        Label = k.Key.Day.ToString()   // x 轴：日
                    })
                    .ToList();

                var visitEntries = VisitMap.OrderBy(k => k.Key)
                    .Select(k => new ChartEntry(k.Value)
                    {
                        ValueLabel = k.Value.ToString(),
                        Color = SKColors.OrangeRed,
                        Label = k.Key.Day.ToString()
                    })
                    .ToList();

                // 2. 包装成 ChartSerie（0.9.5.x 提供的类型）
                var revenueSerie = new ChartSerie
                {
                    Name = "营收",
                    Entries = revEntries
                };

                var visitSerie = new ChartSerie
                {
                    Name = "人次",
                    Entries = visitEntries
                };

                // 3. 构造 LineChart
                ChartModel = new LineChart
                {
                    LineMode = LineMode.Straight,
                    Series = new[] { revenueSerie, visitSerie },
                    LabelOrientation = Orientation.Horizontal,
                    ValueLabelOrientation = Orientation.Horizontal,
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DoctorDetailVM] 加载失败: {ex.Message}");
            }
        }
    }

}
