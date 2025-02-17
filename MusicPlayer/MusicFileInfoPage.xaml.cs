using System.Diagnostics;
using MusicPlayer.Helpers;
using MusicPlayer.ViewModel;
using Plugin.Maui.Audio;

namespace MusicPlayer;

public partial class MusicFileInfoPage : ContentPage
{
    private MusicFileInfoViewModel _vm;
    private readonly AudioService _audioService;
    private bool _initialised;
    private bool _sliderDragging;
    private IDispatcherTimer _timer;

    public MusicFileInfoPage(MusicFileInfoViewModel vm, IAudioManager audioManager)
	{
		InitializeComponent();
		BindingContext = vm;
        _vm = vm;
        _audioService = new AudioService(audioManager);
        _timer = Dispatcher.CreateTimer();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.UpdateMusicInfo();
        if (!_initialised)
            Init();
    }

    public void Init()
    {
        var sourceFile = MarkdownParser.RefToString(Global.musicInfo.info.SourceFile);
        var musicSourcePath = Preferences.Get("music_player.music_source_path", "");
        musicSourcePath = Path.Combine(musicSourcePath, sourceFile);
        if (!Path.Exists(musicSourcePath))
            return;

        var stream = File.OpenRead(musicSourcePath);
        if (stream == null)
        {
            Debug.WriteLine($"Ошибка! Не удалось прочитать файл музыки '{musicSourcePath}'");
            return;
        }
        _audioService.PlayFromStream(stream);

        _timer.Interval = TimeSpan.FromMilliseconds(100);
        _timer.Tick += OnTimerTickEvent;
        _timer.Start();

        _vm.PlayIcon = MaterialIconsHelper.Pause_circle;
        _vm.RepeatColor = Global.repeatPlay ? Colors.RoyalBlue : Colors.DarkSlateGray;
        _vm.RandomColor = Global.randomPlay ? Colors.RoyalBlue : Colors.DarkSlateGray;

        _initialised = true;
    }

    private string DurationToTime(double duration)
    {
        var seconds = (int)duration % 60;
        var minutes = (int)duration / 60;
        var result = "";
        result += minutes < 10 ? $"0{minutes}" : $"{minutes}";
        result += ":";
        result += seconds < 10 ? $"0{seconds}" : $"{seconds}";
        return result;
    }


    private void OnTimerTickEvent(object? sender, EventArgs e)
    {
        if (_audioService == null)
            return;

        Duration.Text = DurationToTime(_audioService.GetDuration());
        Slider.Maximum = _audioService.GetDuration();
        _audioService.SetIsRepeat(Global.repeatPlay);
        if (!_sliderDragging)
        {
            try
            {
                Slider.Value = _audioService.GetCurrentPosition();
            }
            catch (Exception error)
            {
                Debug.Print($"Ошибка! Слайдер затупил. {error.Message}");
            }
            CurrentPosition.Text = DurationToTime(_audioService.GetCurrentPosition());
        }
        else
        {
            CurrentPosition.Text = DurationToTime(Slider.Value);
        }
    }

    private void ButtonPlay_Pressed(object sender, EventArgs e)
    {
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
    }

    private void ButtonNext_Pressed(object sender, EventArgs e)
    {

    }

    private void RepeatButton_Pressed(object sender, EventArgs e)
    {
        Global.repeatPlay = !Global.repeatPlay;
        _vm.RepeatColor = Global.repeatPlay ? Colors.RoyalBlue : Colors.DarkSlateGray;
    }

    private void RandomButton_Pressed(object sender, EventArgs e)
    {
        Global.randomPlay = !Global.randomPlay;
        _vm.RandomColor = Global.randomPlay ? Colors.RoyalBlue : Colors.DarkSlateGray;
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
        _audioService.Seek(Slider.Value);
        _audioService.Resume();
        _sliderDragging = false;
    }

    private void RankButton_Pressed(object sender, EventArgs e)
    {

    }
}
