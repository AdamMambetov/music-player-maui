using CommunityToolkit.Maui.Storage;

namespace MusicPlayer
{
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
                if (folder != null)
                {
                    UpdateMusicSourcePath(folder.Folder.Path);
                }
            }
            catch (Exception ex)
            {


            }
        }

        private async void OnSelectNoteFolderClicked(object sender, EventArgs e)
        {
            try
            {
                var folder = await FolderPicker.PickAsync(default);
                if (folder != null)
                {
                    UpdateMusicNotePath(folder.Folder.Path);
                }
            }
            catch (Exception ex)
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

        //public async string ReadMarkdownFile(string fileName)
        //{
        //    string contentL = "";

        //    try
        //    {
        //        using (var stream = await FileSystem.OpenAppPackageFileAsync(fileName))
        //        {
        //            using (var reader = new StreamReader(stream))
        //            {
        //                contentL = await reader.ReadToEndAsync();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
        //    }

        //    return contentL;
        //}
    }
}