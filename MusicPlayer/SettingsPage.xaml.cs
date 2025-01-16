using CommunityToolkit.Maui.Storage;

namespace MusicPlayer;

public partial class SettingsPage : ContentPage
{
    const string MUSIC_NOTE_PATH_KEY = "music_player.music_note_path";
    const string MUSIC_SOURCE_PATH_KEY = "music_player.music_source_path";

    public SettingsPage()
    {
        InitializeComponent();

        UpdateMusicNotePath(Preferences.Get(MUSIC_NOTE_PATH_KEY, ""));
        UpdateMusicSourcePath(Preferences.Get(MUSIC_SOURCE_PATH_KEY, ""));
    }

    private async void OnSelectSourceFolderClicked(object sender, EventArgs e)
    {
        try
        {
            var folder = await FolderPicker.PickAsync(default);
            if (folder.Folder != null)
            {
                UpdateMusicSourcePath(folder.Folder.Path);
            }
        }
        catch (Exception)
        {
        }
    }

    private async void OnSelectNoteFolderClicked(object sender, EventArgs e)
    {
        try
        {
            var folder = await FolderPicker.PickAsync(default);
            if (folder.Folder != null)
            {
                UpdateMusicNotePath(folder.Folder.Path);
            }
        }
        catch (Exception)
        {
        }
    }

    private void UpdateMusicSourcePath(string path)
    {
        Preferences.Set(MUSIC_SOURCE_PATH_KEY, path);
        if (Path.Exists(path))
        {
            selectedSource.Text = "Music source path:\n" + path;
        }
        else
        {
            selectedSource.Text = "Select source path!";
        }
    }

    private void UpdateMusicNotePath(string path)
    {
        Preferences.Set(MUSIC_NOTE_PATH_KEY, path);
        if (Path.Exists(path))
        {
            selectedNote.Text = "Music note path:\n" + path;
        }
        else
        {
            selectedNote.Text = "Select note path!";
        }
    }
}
