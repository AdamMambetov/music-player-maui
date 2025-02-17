using System.Diagnostics;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace MusicPlayer;


public class MusicInfoMD
{
    public DateTime created { get; set; }
    public DateTime modified { get; set; }
    public string Name { get; set; }
    public string[] Artists { get; set; }
    public string SourceFile { get; set; }
    public string[] aliases { get; set; }
    public string[] tags { get; set; }
    public string Cover { get; set; }
    public int Year { get; set; }
    public string source { get; set; }
    public string Album { get; set; }

    public const string NAME = "Empty name";
    public const string COVER = "No Album Art.jpg";
    public const string ALBUM = "Empty album";
    public const string ARTIST = "Empty artist";
    public const int YEAR = -1;


    public override string ToString() => $"{created}, {modified}, {Name}, {ArtistsToString()}, {SourceFile}, " +
        $"{aliases}, {tags}, {Cover}, {Year}, {source}, {Album}";

    public override bool Equals(object? obj)
    {
        if (obj == null || obj is not MusicInfoMD)
            return false;
        var other = (MusicInfoMD)obj;

        return Name == other.Name
            && ArtistsToString() == other.ArtistsToString()
            && Album == other.Album
            && Year == other.Year;
    }

    public string ArtistsToString()
    {
        string result = "";
        if (Artists.Length == 0)
            return ARTIST;
        for (int i = 0; i < Artists.Length; i++)
        {
            result += MarkdownParser.RefToString(Artists[i]);
            if (i != Artists.Length - 1)
               result += "; ";
        }
        return result;
    }

}

struct MusicInfo
{
    public MusicInfoMD info { get; set; }
    public string note { get; set; }

    public MusicInfo(in MusicInfoMD info, in string note)
    {
        this.info = info;
        this.note = note;
    }
}

public class MarkdownParser
{

    public static (bool success, MusicInfoMD musicInfo, string content) ParseMarkdownWithYaml(string markdown)
    {
        try
        {
            var deserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .Build();

            // Ищем YAML frontmatter между ---
            var match = Regex.Match(markdown, @"^---\s*\n(.*?)\n---\s*\n(.*)",
                RegexOptions.Singleline);

            if (!match.Success)
                return (false, new MusicInfoMD(), markdown);

            var yaml = match.Groups[1].Value;
            var content = match.Groups[2].Value;

            // Десериализуем YAML в объект
            var musicInfo = deserializer.Deserialize<MusicInfoMD>(yaml);
            if (string.IsNullOrEmpty(musicInfo.Name))
                musicInfo.Name = MusicInfoMD.NAME;
            if (string.IsNullOrEmpty(musicInfo.Cover))
                musicInfo.Cover = MusicInfoMD.COVER;
            if (string.IsNullOrEmpty(musicInfo.Album))
                musicInfo.Album = MusicInfoMD.ALBUM;
             musicInfo.Artists ??= [];

            return (true, musicInfo, content);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка парсинга YAML: {ex.Message}");
            throw;
        }
    }

    public static string RefToString(in string s) => s.Replace("[[", "").Replace("]]", "");
    public static string StringToRef(in string s) => $"[[{s}]]";
}
