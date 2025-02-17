using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MusicPlayer.ViewModel;

public partial class MusicFilesViewModel : ObservableObject
{
    public MusicFilesViewModel()
    {
        Items = new ObservableCollection<MusicFileItem>();
    }

    [ObservableProperty]
    ObservableCollection<MusicFileItem> items;

    public void RescanMusic()
    {
        var notePath = Preferences.Get("music_player.music_note_path", "");
        if (!Path.Exists(notePath))
            return;

        Items.Clear();
        Global.allMusicInfos = [];
        foreach (var file in Directory.GetFiles(notePath))
        {
            var res = MarkdownParser.ParseMarkdownWithYaml(File.ReadAllText(file));
            if (!res.success)
                continue;
            MusicInfoMD info = res.musicInfo;

            string cover = MarkdownParser.RefToString(info.Cover);

            var item = new MusicFileItem(
                info.Name,
                info.ArtistsToString(),
                MarkdownParser.RefToString(info.Album),
                ImageSource.FromFile(Path.Combine(notePath, "Covers", cover)),
                info.Year
            );
            Items.Add(item);
            var musicInfo = new MusicInfo(info, Path.GetFileName(file));
            Global.allMusicInfos = [.. Global.allMusicInfos, musicInfo];
        }
    }

    [RelayCommand]
    async Task Tap(MusicFileItem item)
    {
        foreach (var el in Global.allMusicInfos)
        {
            if (el.info.Name == item.Name
                && el.info.ArtistsToString() == item.Artists
                && el.info.Album == item.Album
                && el.info.Year == item.Year)
            {
                Debug.Print("==================== " + el.note);
                Global.musicInfo = el;
                await Shell.Current.GoToAsync(nameof(MusicFileInfoPage));
                break;
            }
        }
    }
}

public class MusicFileItem
{
    public string Name { get; set; }
    public string Artists { get; set; }
    public string Album { get; set; }
    public ImageSource AlbumCover { get; set; }
    public int Year { get; set; }

    public MusicFileItem(string name, string artists, string album, ImageSource albumCover, int year)
    {
        Name = name;
        Artists = artists;
        Album = album;
        AlbumCover = albumCover;
        Year = year;
    }
}
