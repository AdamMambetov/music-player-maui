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
            });

        builder.Services.AddSingleton(AudioManager.Current);
        builder.Services.AddSingleton<MusicFilesViewModel>();

        builder.Services.AddTransient<MusicFileInfoPage>();
        builder.Services.AddTransient<MusicFileInfoViewModel>();

        return builder.Build();
    }
}
