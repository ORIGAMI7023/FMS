using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FMS.Mobile.Models;
using FMS.Mobile.Services;
using System.Collections.ObjectModel;

namespace FMS.Mobile.ViewModels;

public partial class StatisticsViewModel : ObservableObject
{
    private readonly ApiService _api = new();

    [ObservableProperty]
    private ObservableCollection<ItemTypeStatistics> items = new();

    [ObservableProperty]
    private int selectedYear = DateTime.Today.Year;

    [ObservableProperty]
    private int selectedMonth = DateTime.Today.Month;

    public StatisticsViewModel()
    {
        LoadStatisticsCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadStatisticsAsync()
    {
        var list = await _api.GetMonthlyStatisticsAsync(SelectedYear, SelectedMonth);
        Items = new ObservableCollection<ItemTypeStatistics>(list);
    }
}