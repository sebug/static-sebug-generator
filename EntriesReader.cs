using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace StaticSebugGenerator;

public record BlogEntriesReader
{
    public async IAsyncEnumerable<BlogEntry> GetBlogEntries(string sourceDirectory)
    {
        var archivesFolder = Path.Combine(sourceDirectory, "archives");
        if (!Directory.Exists(archivesFolder))
        {
            throw new Exception("Expected archives folder in the source directory");
        }
        var yearRegex = new Regex("\\d{4}");
        var years = Directory.GetDirectories(archivesFolder)
            .Where(f => yearRegex.IsMatch(Path.GetFileName(f) ?? String.Empty))
            .Order();
        var monthRegex = new Regex("\\d{2}");
        var dayFileRegex = new Regex("(\\d{2})\\.html$");
        foreach (var year in years)
        {
            var months = Directory.EnumerateDirectories(year)
            .Where(f => monthRegex.IsMatch(Path.GetFileName(f)))
            .Order();
            foreach (var month in months)
            {
                var days = Directory.EnumerateFiles(month)
                .Where(f => dayFileRegex.IsMatch(Path.GetFileName(f)))
                .Order();
                foreach (var day in days)
                {
                    var dayMatch = dayFileRegex.Match(Path.GetFileName(day));
                    if (!dayMatch.Success)
                    {
                        throw new Exception("Could not extract information from path " + day);
                    }
                    var yearInt = int.Parse(Path.GetFileName(year));
                    var monthInt = int.Parse(Path.GetFileName(month));
                    var dayInt = int.Parse(dayMatch.Groups[1].Value);
                    var blogEntryDate = new DateOnly(yearInt, monthInt, dayInt);

                    yield return await ReadBlogEntry(blogEntryDate, day);
                }
            }
        }
    }

    private async Task<BlogEntry> ReadBlogEntry(DateOnly date, string path)
    {
        string content = await File.ReadAllTextAsync(path, Encoding.UTF8);
        var doc = new HtmlDocument();
        doc.LoadHtml(content);
        var h2 = doc.DocumentNode.Descendants("h2").FirstOrDefault();
        if (h2 == null)
        {
            throw new Exception("Badly formatted blog entry: " + path);
        }
        int colonIndex = h2.InnerHtml.IndexOf(':');
        if (colonIndex < 0)
        {
            throw new Exception("Expected date part of blog entry title but got " + h2.InnerHtml);
        }
        return new BlogEntry(Date: date,
        Title: h2.InnerHtml.Substring(colonIndex + 1).Trim(),
        Content: content,
        FilePath: path);
    }
}