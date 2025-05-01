using Microsoft.Maui.Controls.PlatformConfiguration;
using MusicPlayer.ViewModel;

namespace MusicPlayer;

public partial class MusicFilesPage : ContentPage
{
    private readonly MusicFilesViewModel _vm;


    public MusicFilesPage(MusicFilesViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
        _vm = vm;
		_vm.IsRefreshing = true;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

#if WINDOWS
        _vm.IsRefreshing = true;
#endif
    }
}
