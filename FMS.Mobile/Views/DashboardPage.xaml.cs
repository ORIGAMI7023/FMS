using FMS.Mobile.ViewModels;

namespace FMS.Mobile.Views;

public partial class DashboardPage : ContentPage
{
    public DashboardPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is DashboardViewModel vm &&
            AppState.LastDoctorMonth is DateTime doctorMonth)
        {
            // 如果 Home 当前月份与医生页不同，则切换并重新加载
            if (vm.SelectedDate.Year != doctorMonth.Year ||
                vm.SelectedDate.Month != doctorMonth.Month)
            {
                vm.SelectedDate = doctorMonth;   // 将触发 OnSelectedDateChanged → 自动 LoadSummaryAsync
            }
        }
    }
}
