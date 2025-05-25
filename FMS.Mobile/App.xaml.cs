using System.Globalization;

namespace FMS.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            CultureInfo.CurrentCulture = new CultureInfo("zh-CN");
            CultureInfo.CurrentUICulture = new CultureInfo("zh-CN");

            MainPage = new AppShell();
        }
    }
}
