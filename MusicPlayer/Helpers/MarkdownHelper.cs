using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace MusicPlayer.Helpers;


public partial class MarkdownHelper
{
    // Какая-то крутая функция, которая сама генерируется. Она ищет YAML frontmatter между ---
    // https://learn.microsoft.com/ru-ru/dotnet/standard/base-types/regular-expression-source-generators
    [GeneratedRegex(@"^---\s*\n(.*?)\n---\s*\n(.*)", RegexOptions.Singleline)]
    private static partial Regex YamlFrontmatterGeneratedRegex();

    public static (bool success, MusicInfoMD musicInfo, string content) ParseMarkdownWithYaml(string markdown)
    {
        try
        {
            var deserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .Build();

            // Ищем YAML frontmatter между ---
            var match = YamlFrontmatterGeneratedRegex().Match(markdown);
            if (!match.Success)
                return (false, new MusicInfoMD(), markdown);

            var yaml = match.Groups[1].Value;
            var content = match.Groups[2].Value;

            // Десериализуем YAML в объект
            var musicInfo = deserializer.Deserialize<MusicInfoMD>(yaml);
            return (true, musicInfo, content);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка парсинга YAML: {ex.Message}");
            throw;
        }
    }

}


public class MusicInfoMD
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public DateTime created { get; set; }
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public DateTime modified { get; set; }
    public string Name { get; set; }
    public string[] creator { get; set; }
    public string SourceFile { get; set; }
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public string[] aliases { get; set; }
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public string[] tags { get; set; }
    public string Cover { get; set; }
    public int Year { get; set; }
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public string source { get; set; }
    public string Album { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public const string EMPTY_NAME = "Empty name";
    public const string EMPTY_ALBUM = "Empty album";
    public const string EMPTY_ARTIST = "Empty artist";
    public const int EMPTY_YEAR = 0;


    public override string ToString() => $"{created}, {modified}, {Name}, {creator.ToStr()}, {SourceFile}, " +
        $"{aliases.ToStr()}, {tags?.ToStr()}, {Cover}, {Year}, {source}, {Album}";

}


public static class StringExtenstions
{
    public static string RefToString(this string s) => s == null ? "" : s.Replace("[[", "").Replace("]]", "");

    public static string StringToRef(this string s) => $"[[{s}]]";

    public static void FromRef(ref string s)
    {
        s = s.Replace("[[", "").Replace("]]", "");
    }

    public static void ToRef(ref string s)
    {
        s = $"[[{s}]]";
    }
}

public static class ArrayExtensions
{
    public static string ToStr<T>(this T[] array, in string separator = "; ")
    {
        return string.Join(separator, array);
    }
}
