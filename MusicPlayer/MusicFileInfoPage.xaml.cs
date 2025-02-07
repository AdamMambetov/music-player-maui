using System.Diagnostics;
using MusicPlayer.ViewModel;
using Plugin.Maui.Audio;

namespace MusicPlayer;

public partial class MusicFileInfoPage : ContentPage
{
    private MusicFileInfoViewModel _vm;
    private readonly AudioService _audioService;
    private bool _initialised;

    public MusicFileInfoPage(MusicFileInfoViewModel vm, IAudioManager audioManager)
	{
		InitializeComponent();
		BindingContext = vm;
        _vm = vm;
        _audioService = new AudioService(audioManager);
    }


    public void Init()
    {
        if (_vm.MusicFileInfo == null)
            return;

        var sourceFile = _vm.MusicFileInfo.SourceFile.Replace("[[", "").Replace("]]", "");
        var musicSourcePath = Preferences.Get("music_player.music_source_path", "");
        if (!Path.Exists($"{musicSourcePath}/{sourceFile}"))
            return;

        var stream = File.OpenRead($"{musicSourcePath}/{sourceFile}");
        if (stream == null)
        {
            Debug.WriteLine($"Ошибка! Не удалось прочитать файл музыки '{musicSourcePath}/{sourceFile}'");
            return;
        }
        _audioService.PlayFromStream(stream);
        Duration.Text = $"{((int)_audioService.GetDuration()) / 60}:{((int)_audioService.GetDuration()) % 60}";
    }


    private void ContentPage_Appearing(object sender, EventArgs e)
    {
        _vm.UpdateMusicInfo();
        if (!_initialised)
            Init();
    }

    private void ButtonPlay_Pressed(object sender, EventArgs e)
    {
        if (_audioService.GetIsPlaying())
            _audioService.Pause();
        else
            _audioService.Resume();
        Duration.Text = $"{((int)_audioService.GetDuration()) / 60}:{((int)_audioService.GetDuration()) % 60}";
    }

    private void ButtonPrev_Pressed(object sender, EventArgs e)
    {
    }

    private void ButtonNext_Pressed(object sender, EventArgs e)
    {

    }

    private void ButtonRepeat_Pressed(object sender, EventArgs e)
    {
        _audioService.SetIsRepeat(!_audioService.GetIsRepeat());
    }

    private void ButtonRandom_Pressed(object sender, EventArgs e)
    {

    }

    private void ButtonFavourite_Pressed(object sender, EventArgs e)
    {

    }

}