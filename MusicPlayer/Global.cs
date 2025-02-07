namespace MusicPlayer;

class Global
{
    private static Global _instance;

    public MusicInfo musicInfo { get; set; }


    Global()
    {
        _instance = this;
    }

    public static Global Get()
    {
        return _instance;
    }
}
