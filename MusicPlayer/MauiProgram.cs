using CommunityToolkit.Maui;
using MusicPlayer.ViewModel;
using Plugin.Maui.Audio;

namespace MusicPlayer;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

                // Font Awesome Icons
                fonts.AddFont("Brands-Regular-400.otf", "FAB");
                fonts.AddFont("Free-Regular-400.otf", "FAR");
                fonts.AddFont("Free-Solid-900.otf", "FAS");
            });

        builder.Services.AddSingleton(AudioManager.Current);
        builder.Services.AddSingleton<MusicFilesViewModel>();

        builder.Services.AddTransient<MusicFileInfoPage>();
        builder.Services.AddTransient<MusicFileInfoViewModel>();

        return builder.Build();
    }
}
