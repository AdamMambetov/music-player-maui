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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.RescanMusic();
    }
}
