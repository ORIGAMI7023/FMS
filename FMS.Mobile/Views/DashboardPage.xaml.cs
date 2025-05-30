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
            // ��� Home ��ǰ�·���ҽ��ҳ��ͬ�����л������¼���
            if (vm.SelectedDate.Year != doctorMonth.Year ||
                vm.SelectedDate.Month != doctorMonth.Month)
            {
                vm.SelectedDate = doctorMonth;   // ������ OnSelectedDateChanged �� �Զ� LoadSummaryAsync
            }
        }
    }
}
