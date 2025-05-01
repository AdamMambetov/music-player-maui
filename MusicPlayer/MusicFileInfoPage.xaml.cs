using System.Diagnostics;
using MusicPlayer.Helpers;
using MusicPlayer.ViewModel;
using Plugin.Maui.Audio;

namespace MusicPlayer;

public partial class MusicFileInfoPage : ContentPage
{
    private readonly MusicFileInfoViewModel _vm;
    private readonly AudioService _audioService;
    private readonly IDispatcherTimer _timer;
    private bool _sliderDragging;


    public MusicFileInfoPage(MusicFileInfoViewModel vm, IAudioManager audioManager)
	{
		InitializeComponent();
		BindingContext = vm;
        _vm = vm;
        _audioService = new AudioService(audioManager);
        _timer = Dispatcher.CreateTimer();

        Init();
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.UpdateMusicInfo();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _timer.Stop();
        _audioService.Dispose();
    }


    private void Init()
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
        if (_audioService.IsAudioPlayerValid)
            _audioService.UnbindFromEndedEvent(OnAudioEnded);
        _audioService.PlayFromStream(stream);
        _audioService.BindToEndedEvent(OnAudioEnded);

        if (!_timer.IsRunning)
        {
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += OnTimerTickEvent;
            _timer.Start();
        }

        _vm.PlayIcon = MaterialIconsHelper.Pause_circle;
        _vm.RepeatColor = Global.RepeatPlay ? Colors.RoyalBlue : Colors.DarkSlateGray;
        _vm.RandomColor = Global.RandomPlay ? Colors.RoyalBlue : Colors.DarkSlateGray;

        _vm.UpdateMusicInfo();
    }

    private static string DurationToTime(double duration)
    {
        var seconds = (int)duration % 60;
        var minutes = (int)duration / 60;
        var result = "";
        result += minutes < 10 ? $"0{minutes}" : $"{minutes}";
        result += ":";
        result += seconds < 10 ? $"0{seconds}" : $"{seconds}";
        return result;
    }


    private void OnAudioEnded(object? sender, EventArgs e)
    {
        if (Global.RepeatPlay)
            _audioService.Seek(0);
        else
            ButtonNext_Pressed(sender, e);
    }

    private void OnTimerTickEvent(object? sender, EventArgs e)
    {
        if (!_audioService.IsAudioLoaded)
            return;

        Duration.Text = DurationToTime(_audioService.GetDuration());
        _audioService.SetIsRepeat(Global.RepeatPlay);

        if (!_sliderDragging)
            Slider.Value = _audioService.GetCurrentPosition() / _audioService.GetDuration();
        else
            CurrentPosition.Text = DurationToTime(Slider.Value * _audioService.GetDuration());
        CurrentPosition.Text = DurationToTime(_audioService.GetCurrentPosition());
    }

    private void ButtonPlay_Pressed(object sender, EventArgs e)
    {
        if (!_audioService.IsAudioLoaded)
            return;

        if (_audioService.GetIsPlaying())
        {
            _audioService.Pause();
            _vm.PlayIcon = MaterialIconsHelper.Play_circle;
        }
        else
        {
            _audioService.Resume();
            _vm.PlayIcon = MaterialIconsHelper.Pause_circle;
        }
    }

    private void ButtonPrev_Pressed(object sender, EventArgs e)
    {
        var index = Array.FindIndex(Global.MusicQueue, (info) => info.Note == Global.MusicInfo.Note);
        if (index == 0)
            return;
        Global.MusicInfo = Global.MusicQueue[index - 1];
        Init();
    }

    private void ButtonNext_Pressed(object? sender, EventArgs e)
    {
        var index = Array.FindIndex(Global.MusicQueue, (info) => info.Note == Global.MusicInfo.Note);
        if (index == Global.MusicQueue.Length - 1)
            return;
        Global.MusicInfo = Global.MusicQueue[index + 1];
        Init();
    }

    private void RepeatButton_Pressed(object sender, EventArgs e)
    {
        Global.RepeatPlay = !Global.RepeatPlay;
        _vm.RepeatColor = Global.RepeatPlay ? Colors.RoyalBlue : Colors.DarkSlateGray;
    }

    private void RandomButton_Pressed(object sender, EventArgs e)
    {
        Global.RandomPlay = !Global.RandomPlay;
        _vm.RandomColor = Global.RandomPlay ? Colors.RoyalBlue : Colors.DarkSlateGray;
    }

    private void ButtonFavourite_Pressed(object sender, EventArgs e)
    {

    }

    private void Slider_DragStarted(object sender, EventArgs e)
    {
        _sliderDragging = true;
        _audioService.Pause();
    }

    private void Slider_DragCompleted(object sender, EventArgs e)
    {
        _sliderDragging = false;

        if (!_audioService.IsAudioLoaded)
            return;

        _audioService.Seek(Slider.Value * _audioService.GetDuration());
        _audioService.Resume();
    }

    private void RankButton_Pressed(object sender, EventArgs e)
    {

    }

}
