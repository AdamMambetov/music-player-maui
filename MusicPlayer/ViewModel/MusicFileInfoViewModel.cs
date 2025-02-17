using CommunityToolkit.Mvvm.ComponentModel;

namespace MusicPlayer.ViewModel;

public partial class MusicFileInfoViewModel : ObservableObject
{
    [ObservableProperty]
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



    public void UpdateMusicInfo()
    {
        var info = Global.musicInfo.info;

        Name = info.Name;
        Artists = info.ArtistsToString();

        MusicInfo = $"""
        Created: {info.created}
        Modified: {info.modified}
        Artists: {Artists}
        Album: {MarkdownParser.RefToString(info.Album)}
        SourceFile: {MarkdownParser.RefToString(info.SourceFile)}
        """;
    }
}
