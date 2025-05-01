using CommunityToolkit.Mvvm.ComponentModel;
using MusicPlayer.Helpers;

namespace MusicPlayer.ViewModel;

public partial class MusicFileInfoViewModel : ObservableObject
{
    [ObservableProperty]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    string name;

    [ObservableProperty]
    string artists;

    [ObservableProperty]
    string musicInfo;

    [ObservableProperty]
    string playIcon;

    [ObservableProperty]
    Color repeatColor;

    [ObservableProperty]
    Color randomColor;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.


    public void UpdateMusicInfo()
    {
        Name = Global.MusicInfo.Info.Name;
        Artists = Global.MusicInfo.ArtistsString;

        MusicInfo = $"""
        Created: {Global.MusicInfo.Info.created}
        Modified: {Global.MusicInfo.Info.modified}
        Artists: {Artists}
        Album: {Global.MusicInfo.AlbumString}
        SourceFile: {Global.MusicInfo.Info.SourceFile.RefToString()}
        """;
    }
}
