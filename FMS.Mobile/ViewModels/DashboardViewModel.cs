using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FMS.Mobile.Models;
using FMS.Mobile.Services;
using System.Collections.ObjectModel;

namespace FMS.Mobile.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly ApiService _api;

    [ObservableProperty]
    private ObservableCollection<RevenueRecord> records = new();

    [ObservableProperty]
    private DateOnly selectedDate = DateOnly.FromDateTime(DateTime.Today);

    public DashboardViewModel()
    {
        _api = new ApiService();
        LoadDataCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        var data = await _api.GetDailyRevenueAsync(SelectedDate);
        Records = new ObservableCollection<RevenueRecord>(data);
    }
}