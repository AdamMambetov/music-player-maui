using System.Diagnostics;
using Microsoft.Maui.Controls;
using MusicPlayer.Helpers;
using MusicPlayer.ViewModel;
using Plugin.Maui.Audio;

namespace MusicPlayer;

public partial class MusicFileInfoPage : ContentPage
{
    private readonly MusicFileInfoViewModel _vm;
    private readonly AudioService AudioService;


    public MusicFileInfoPage(MusicFileInfoViewModel vm, IAudioManager audioManager, INotificationManagerService notificationManager)
	{
		InitializeComponent();
		BindingContext = vm;
        _vm = vm;
        _vm.AudioService = new AudioService(audioManager);
        _vm.Timer = Dispatcher.CreateTimer();
        _vm.NotificationManager = notificationManager;
        _vm.NotificationManager.SendNotification("", "");

        _vm.Init();
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.UpdateMusicInfo();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _vm.ClosePlayer();
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


    private void OnTimerTickEvent(object? sender, EventArgs e)
    {
        if (!_vm.AudioService.IsAudioLoaded)
            return;

        Duration.Text = DurationToTime(_vm.AudioService.GetDuration());
        _vm.AudioService.SetIsRepeat(Global.RepeatPlay);

        if (!_vm.SliderDragging)
            Slider.Value = _vm.AudioService.GetCurrentPosition() / _vm.AudioService.GetDuration();
        else
            CurrentPosition.Text = DurationToTime(Slider.Value * _vm.AudioService.GetDuration());
        CurrentPosition.Text = DurationToTime(_vm.AudioService.GetCurrentPosition());
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
        _vm.SliderDragging = true;
        _vm.AudioService.Pause();
    }

    private void Slider_DragCompleted(object sender, EventArgs e)
    {
        _vm.SliderDragging = false;

        if (!_vm.AudioService.IsAudioLoaded)
            return;

        _vm.AudioService.Seek(Slider.Value * _vm.AudioService.GetDuration());
        _vm.AudioService.Resume();
    }

    private void RankButton_Pressed(object sender, EventArgs e)
    {

    }

}
