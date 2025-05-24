
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
    private ObservableCollection<MonthlyAmountStatistics> items = new();

    [ObservableProperty]
    private DateTime selectedDate = DateTime.Today;

    public StatisticsViewModel()
    {
        LoadStatisticsCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadStatisticsAsync()
    {
        var list = await _api.GetMonthlyAmountStatisticsAsync(SelectedDate.Year, SelectedDate.Month);
        Items = new ObservableCollection<MonthlyAmountStatistics>(list ?? new List<MonthlyAmountStatistics>());
    }
}
