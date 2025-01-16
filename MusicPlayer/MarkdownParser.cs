using System.Diagnostics;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace MusicPlayer;

public class MarkdownParser
{
    public class MusicInfo
    {
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public string Name { get; set; }
        public string[] artists { get; set; }
        public string SourceFile { get; set; }

        override public string ToString() => $"{created}, {modified}, {Name}, {artists}, {SourceFile}";
    }

    public (MusicInfo? musicInfo, string content) ParseMarkdownWithYaml(string markdown)
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
                return (null, markdown);

            var yaml = match.Groups[1].Value;
            var content = match.Groups[2].Value;

            // Десериализуем YAML в объект
            var musicInfo = deserializer.Deserialize<MusicInfo>(yaml);
            return (musicInfo, content);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка парсинга YAML: {ex.Message}");
            throw;
        }
    }
}
