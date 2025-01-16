using MusicPlayer.ViewModel;
using Plugin.Maui.Audio;

namespace MusicPlayer;

public partial class MusicFileInfoPage : ContentPage
{
    private MusicFileInfoViewModel _vm;
    private readonly AudioService _audioService;

    public MusicFileInfoPage(MusicFileInfoViewModel vm, IAudioManager audioManager)
	{
		InitializeComponent();
		BindingContext = vm;
        _vm = vm;
        _audioService = new AudioService(audioManager);
	}

    private void ContentPage_Appearing(object sender, EventArgs e)
    {
        _vm.UpdateMusicInfo();
    }

    private void Button_Pressed(object sender, EventArgs e)
    {
        if (_vm.MusicFileInfo == null)
            return;
        
        var sourceFile = _vm.MusicFileInfo.SourceFile.Replace("[[", "").Replace("]]", "");
        var musicSourcePath = Preferences.Get("music_player.music_source_path", "");
        if (!Path.Exists($"{musicSourcePath}/{sourceFile}"))
            return;

        var stream = File.OpenRead($"{musicSourcePath}/{sourceFile}");
        if (stream == null ) return;
        _audioService.PlayFromStream(stream);
    }
}