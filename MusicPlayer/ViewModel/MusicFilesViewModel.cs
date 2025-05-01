using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MusicPlayer.Helpers;

namespace MusicPlayer.ViewModel;

public partial class MusicFilesViewModel : ObservableObject
{
    [ObservableProperty]
    ObservableCollection<MusicInfo> items;

    [ObservableProperty]
    bool isRefreshing;
    private readonly BackgroundWorker RescanMusicWorker = new BackgroundWorker();

    public MusicFilesViewModel()
    {
        Items = [];
    }


    [RelayCommand]
    async Task Tap(MusicInfo item)
    {
        foreach (var el in Global.AllMusicInfos)
        {
            if (el.Note == item.Note)
            {
                Global.MusicInfo = el;
                await Shell.Current.GoToAsync(nameof(MusicFileInfoPage));
                break;
            }
        }
    }

    [RelayCommand]
    public void RescanMusic()
    {
        if (RescanMusicWorker.IsBusy)
            return;

        if (!Path.Exists(Global.NotePath))
        {
            IsRefreshing = false;
            return;
        }

        Global.NoAlbumArtCover ??= ImageSource.FromFile(Global.NoAlbumArtCoverPath);

        var TempItems = new List<MusicInfo>();
        RescanMusicWorker.DoWork += (s, e) =>
        {
            foreach (var file in Directory.GetFiles(Global.NotePath))
            {
                var res = MarkdownHelper.ParseMarkdownWithYaml(File.ReadAllText(file));
                if (!res.success)
                    continue;
                MusicInfoMD info = res.musicInfo;

                ImageSource imageSource = string.IsNullOrEmpty(info.Cover.RefToString())
                    ? imageSource = Global.NoAlbumArtCover
                    : imageSource = ImageSource.FromFile(Path.Combine(Global.CoversPath, info.Cover.RefToString()));
                
                if (string.IsNullOrEmpty(info.Name))
                    info.Name = Path.GetFileNameWithoutExtension(info.SourceFile.RefToString());

                var musicInfo = new MusicInfo(
                    info: info,
                    note: Path.GetFileName(file),
                    albumCover: imageSource
                );
                TempItems.Add(musicInfo);
            }
        };
        RescanMusicWorker.RunWorkerCompleted += (s, e) =>
        {
            Global.AllMusicInfos = TempItems.ToArray();
            Global.UpdateMusicQueue(EMusicProperty.Created, descending: true);
            Items = Global.MusicQueue.ToObservableCollection();
            IsRefreshing = false;
        };
        RescanMusicWorker.RunWorkerAsync();
    }
}

public struct MusicInfo
{
    public MusicInfoMD Info { get; set; }
    public string Note { get; set; }
    public string ArtistsString { get; set; }
    public string AlbumString { get; set; }
    public ImageSource AlbumCover { get; set; }

    public MusicInfo(in MusicInfoMD info, in string note, in ImageSource albumCover)
    {
        Info = info;
        Note = note;
        ArtistsString = ArtistsToString();
        AlbumString = Info.Album.RefToString();
        AlbumCover = albumCover;
    }


    public readonly string ArtistsToString()
    {
        string result = "";
        foreach (var artist in Info.Artists)
            result += $"{artist.RefToString()}; ";
        return result.TrimEnd("; ".ToCharArray());
    }
}
