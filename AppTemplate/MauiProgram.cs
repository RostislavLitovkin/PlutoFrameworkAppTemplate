using CommunityToolkit.Maui;
using PlutoFramework;

#if ANDROID26_0_OR_GREATER
using PlutoFramework.Platforms.Android;
#endif

namespace AppTemplate
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {

#if ANDROID26_0_OR_GREATER
            AndroidNotificationHelper.AppIcon = CommunityToolkit.Maui.Core.Resource.Drawable.resourceappicon;
            AndroidNotificationHelper.MainActivityType = typeof(Platforms.Android.MainActivity);
#endif

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UsePlutoFramework();

            return builder.Build();
        }
    }
}
