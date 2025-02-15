using CommunityToolkit.Mvvm.ComponentModel;

namespace MusicPlayer.ViewModel;

[QueryProperty("FileName", "FileName")]
public partial class MusicFileInfoViewModel : ObservableObject
{
    [ObservableProperty]
    string fileName;

    [ObservableProperty]
    string musicInfo;

    [ObservableProperty]
    string playIcon;

    [ObservableProperty]
    Color repeatColor;

    [ObservableProperty]
    Color randomColor;

    public MusicInfoMD? MusicFileInfo;


    public void UpdateMusicInfo()
    {
        var musicPath = Preferences.Get("music_player.music_note_path", "");
        if (!Path.Exists(musicPath))
            return;

        var info = File.ReadAllText($"{musicPath}/{FileName}");
        MusicFileInfo = MarkdownParser.ParseMarkdownWithYaml(info).musicInfo;
        if (MusicFileInfo != null)
        {
            MusicInfo = $"""
            Name: {MusicFileInfo.Name}
            Created: {MusicFileInfo.created}
            Modified: {MusicFileInfo.modified}
            Artists: {MusicFileInfo.artists.GetValue(0)}
            SourceFile: {MusicFileInfo.SourceFile}
            """;
        }
    }
}
