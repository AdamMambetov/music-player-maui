using MusicPlayer.ViewModel;

namespace MusicPlayer;

public partial class MusicFilesPage : ContentPage
{
    MusicFilesViewModel _vm;


    public MusicFilesPage(MusicFilesViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
        _vm = vm;
	}

    private void ContentPage_Appearing(object sender, EventArgs e)
    {
        _vm.RescanMusic();
    }
}