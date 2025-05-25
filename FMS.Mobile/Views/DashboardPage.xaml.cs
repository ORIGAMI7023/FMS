
using FMS.Mobile.ViewModels;

namespace FMS.Mobile.Views;

public partial class DashboardPage : ContentPage
{
    public DashboardPage()
    {
        InitializeComponent();
        Loaded += async (s, e) =>
        {
            if (BindingContext is DashboardViewModel vm)
            {
                await vm.LoadSummaryCommand.ExecuteAsync(null);
            }
        };
    }
}
