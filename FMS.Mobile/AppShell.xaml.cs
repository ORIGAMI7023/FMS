namespace FMS.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("doctorDetail", typeof(Views.DoctorDetailPage));

    }
}