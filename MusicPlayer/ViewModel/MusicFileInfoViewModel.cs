using CommunityToolkit.Mvvm.ComponentModel;

namespace MusicPlayer.ViewModel;

[QueryProperty("FileName", "FileName")]
public partial class MusicFileInfoViewModel : ObservableObject
{
    [ObservableProperty]
    string fileName;

    [ObservableProperty]
    string musicInfo;

    public MarkdownParser.MusicInfo? MusicFileInfo;


    public void UpdateMusicInfo()
    {
        var musicPath = Preferences.Get("music_player.music_note_path", "");
        if (Path.Exists(musicPath))
        {
            var info = File.ReadAllText($"{musicPath}/{FileName}");
            var result = new MarkdownParser().ParseMarkdownWithYaml(info);
            MusicFileInfo = result.musicInfo;
            if (result.musicInfo != null)
            {
                MusicInfo = $"""
                Name: {result.musicInfo.Name}
                Created: {result.musicInfo.created}
                Modified: {result.musicInfo.modified}
                Artists: {result.musicInfo.artists.GetValue(0)}
                SourceFile: {result.musicInfo.SourceFile}

                {result.content}
                """;
            }
        }
    }
}
