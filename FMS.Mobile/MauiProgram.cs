using CommunityToolkit.Maui;
using Microcharts.Maui;
using Microsoft.Extensions.Logging;

namespace FMS.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                   .UseMauiApp<App>()
                   .UseMicrocharts()
                   .UseMauiCommunityToolkit()
                   .ConfigureFonts(fonts =>
                   {
                       fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                       fonts.AddFont("OpenSans-SemiBold.ttf", "OpenSansSemiBold");
                       fonts.AddFont("STXIHEI.TTF", "ChineseFont");
                   });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
