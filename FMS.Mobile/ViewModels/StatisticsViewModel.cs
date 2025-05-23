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
    private DateTime selectedDate = DateTime.Today;

    public StatisticsViewModel()
    {
        LoadStatisticsCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadStatisticsAsync()
    {
        var list = await _api.GetStatisticsAsync(SelectedDate);
        Items = new ObservableCollection<ItemTypeStatistics>(list ?? new List<ItemTypeStatistics>());
    }
}
