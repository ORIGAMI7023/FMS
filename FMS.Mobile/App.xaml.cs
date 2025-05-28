using System.Diagnostics;
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

            //全局异常捕获
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                Debug.WriteLine($"[全局异常]：{ex?.Message}");
            };


            MainPage = new AppShell();
        }
    }
}
