using FMS.Mobile.ViewModels;

namespace FMS.Mobile.Views;

[QueryProperty(nameof(Owner), "owner")]
[QueryProperty(nameof(MonthParam), "month")]
public partial class DoctorDetailPage : ContentPage
{
    public string Owner { get; set; } = "";
    public DateTime MonthParam { get; set; }

    public DoctorDetailPage() { InitializeComponent(); }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        BindingContext = new DoctorDetailViewModel(Owner, MonthParam);
    }
}