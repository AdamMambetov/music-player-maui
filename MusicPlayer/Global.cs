using MusicPlayer.ViewModel;

namespace MusicPlayer;

static class Global
{
    public static MusicInfo[] AllMusicInfos { get; set; } = [];
    public static string[] MusicNotesQueue { get; set; } = [];
    public static MusicInfo MusicInfo { get; set; } = new MusicInfo();
    public static bool RandomPlay
    { 
        get => Preferences.Get("music_player.random_play", false);
        set => Preferences.Set("music_player.random_play", value);
    }
    public static bool RepeatPlay
    {
        get => Preferences.Get("music_player.repeat_play", false);
        set => Preferences.Set("music_player.repeat_play", value);
    }
    public static ImageSource? NoAlbumArtCover { get; set; } = null;
    public static string NoAlbumArtCoverName { get; } = "_No Album Art.jpg";
    public static string NotePath
    {
        get => Preferences.Get("music_player.music_note_path", "");
    }
    public static string MusicPath
    {
        get => Preferences.Get("music_player.music_source_path", "");
    }
    public static string CoversPath
    {
        get => Path.Combine(NotePath, "Covers");
    }
    public static string NoAlbumArtCoverPath
    {
        get => Path.Combine(CoversPath, NoAlbumArtCoverName);
    }
    public static EMusicProperty SortKey
    {
        get => (EMusicProperty)Preferences.Get("music_player.sort_key", (int)EMusicProperty.Created);
        set => Preferences.Set("music_player.sort_key", (int)value);
    }
    public static bool SortDescending
    {
        get => Preferences.Get("music_player.sort_descending", true);
        set => Preferences.Set("music_player.sort_descending", value);
    }


    public static void UpdateMusicQueue(EMusicProperty sortKey, bool descending = false)
    {
        SortKey = sortKey;
        SortDescending = descending;

        //switch (sortKey)
        //{
        //    case EMusicProperty.Name:
        //        MusicQueue = descending
        //            ? AllMusicInfos.OrderByDescending((music) => music.Info.Name).ToArray()
        //            : AllMusicInfos.OrderBy((music) => music.Info.Name).ToArray();
        //        break;
        //    case EMusicProperty.Created:
        //        MusicQueue = descending
        //            ? AllMusicInfos.OrderByDescending((music) => music.Info.created).ToArray()
        //            : AllMusicInfos.OrderBy((music) => music.Info.created).ToArray();
        //        break;
        //    case EMusicProperty.Modified:
        //        MusicQueue = descending
        //            ? AllMusicInfos.OrderByDescending((music) => music.Info.modified).ToArray()
        //            : AllMusicInfos.OrderBy((music) => music.Info.modified).ToArray();
        //        break;
        //    case EMusicProperty.Artist:
        //        MusicQueue = descending
        //            ? AllMusicInfos.OrderByDescending((music) => music.ArtistsString).ToArray()
        //            : AllMusicInfos.OrderBy((music) => music.ArtistsString).ToArray();
        //        break;
        //    case EMusicProperty.Album:
        //        MusicQueue = descending
        //            ? AllMusicInfos.OrderByDescending((music) => music.AlbumString).ToArray()
        //            : AllMusicInfos.OrderBy((music) => music.AlbumString).ToArray();
        //        break;
        //}
    }
}


public enum EMusicProperty
{
    Name, Created, Modified, Artist, Album
}
