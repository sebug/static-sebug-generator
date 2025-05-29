using HtmlAgilityPack;

namespace StaticSebugGenerator;

public record OutputGenerator(TemplateContent TemplateContent, NavContent NavContent,
    string OutputDirectory)
{
    public GeneratedOutput GenerateIndividualBlogEntry(BlogEntry blogEntry)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(TemplateContent.Content);

        string outputPath = Path.Combine(OutputDirectory,
        blogEntry.Date.Year.ToString(),
        blogEntry.Date.Month.ToString("D2"),
        blogEntry.Date.Day.ToString("D2"),
        "index.html");

        return new GeneratedOutput(outputPath, doc.DocumentNode.OuterHtml);
    }
}