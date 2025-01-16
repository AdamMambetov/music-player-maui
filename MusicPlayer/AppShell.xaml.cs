namespace MusicPlayer
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(MusicFileInfoPage), typeof(MusicFileInfoPage));
        }
    }
}
