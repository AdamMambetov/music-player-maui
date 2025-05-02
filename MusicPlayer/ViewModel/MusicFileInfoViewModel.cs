using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MusicPlayer.Helpers;
using System.Diagnostics;

namespace MusicPlayer.ViewModel;

public partial class MusicFileInfoViewModel : ObservableObject
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
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

    public AudioService AudioService;

    [ObservableProperty]
    IDispatcherTimer timer;
    
    [ObservableProperty]
    INotificationManagerService notificationManager;
    
    [ObservableProperty]
    bool sliderDragging;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.


    [RelayCommand]
    public void Play()
    {
        if (!AudioService.IsAudioLoaded)
            return;

        if (AudioService.GetIsPlaying())
        {
            AudioService.Pause();
            PlayIcon = MaterialIconsHelper.Play_circle;
        }
        else
        {
            AudioService.Resume();
            PlayIcon = MaterialIconsHelper.Pause_circle;
        }
    }

    [RelayCommand]
    public void Next()
    {
        if (Global.MusicNotesQueue.Length > 0)
        {
            var index = Array.IndexOf(Global.MusicNotesQueue, Global.MusicInfo.Note);
            if (index == Global.MusicNotesQueue.Length - 1)
                return;
            Global.MusicInfo = Array.Find(
                Global.AllMusicInfos,
                (info) => info.Note == Global.MusicNotesQueue[index + 1]
            );
        }
        else
        {
            var index = Array.FindIndex(Global.AllMusicInfos, (info) => info.Note == Global.MusicInfo.Note);
            if (index == Global.AllMusicInfos.Length - 1)
                return;
            Global.MusicInfo = Global.AllMusicInfos[index + 1];
        }
        Init();
    }

    [RelayCommand]
    public void Prev()
    {
        if (Global.MusicNotesQueue.Length > 0)
        {
            var index = Array.IndexOf(Global.MusicNotesQueue, Global.MusicInfo.Note);
            if (index == 0)
                return;
            Global.MusicInfo = Array.Find(
                Global.AllMusicInfos,
                (info) => info.Note == Global.MusicNotesQueue[index - 1]
            );
        }
        else
        {
            var index = Array.FindIndex(Global.AllMusicInfos, (info) => info.Note == Global.MusicInfo.Note);
            if (index == 0)
                return;
            Global.MusicInfo = Global.AllMusicInfos[index - 1];
        }
        Init();
    }


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

    public void Init()
    {
        var sourceFilePath = Path.Combine(Global.MusicPath, Global.MusicInfo.Info.SourceFile.RefToString());
        if (!Path.Exists(sourceFilePath))
            return;

        var stream = File.OpenRead(sourceFilePath);
        if (stream == null)
        {
            Debug.WriteLine($"Ошибка! Не удалось прочитать файл музыки '{sourceFilePath}'");
            return;
        }
        if (AudioService.IsAudioPlayerValid)
            AudioService.UnbindFromEndedEvent(OnAudioEnded);
        AudioService.PlayFromStream(stream);
        AudioService.BindToEndedEvent(OnAudioEnded);

        if (!Timer.IsRunning)
        {
            Timer.Interval = TimeSpan.FromMilliseconds(100);
            Timer.Tick += OnTimerTickEvent;
            Timer.Start();
        }

        PlayIcon = MaterialIconsHelper.Pause_circle;
        RepeatColor = Global.RepeatPlay ? Colors.RoyalBlue : Colors.DarkSlateGray;
        RandomColor = Global.RandomPlay ? Colors.RoyalBlue : Colors.DarkSlateGray;

        UpdateMusicInfo();
    }

    public void ClosePlayer()
    {
        AudioService.UnbindFromEndedEvent(OnAudioEnded);
        Timer.Stop();
        AudioService.Dispose();
    }

    public void OnAudioEnded(object? sender, EventArgs e)
    {
        if (Global.RepeatPlay)
            AudioService.Seek(0);
        else
            Next();
    }

    private void OnTimerTickEvent(object? sender, EventArgs e)
    {
        if (!AudioService.IsAudioLoaded)
            return;

        Duration.Text = DurationToTime(_vm.AudioService.GetDuration());
        _vm.AudioService.SetIsRepeat(Global.RepeatPlay);

        if (!_vm.SliderDragging)
            Slider.Value = _vm.AudioService.GetCurrentPosition() / _vm.AudioService.GetDuration();
        else
            CurrentPosition.Text = DurationToTime(Slider.Value * _vm.AudioService.GetDuration());
        CurrentPosition.Text = DurationToTime(_vm.AudioService.GetCurrentPosition());
    }
}
