using HtmlAgilityPack;

namespace StaticSebugGenerator;

public record OutputGenerator(TemplateContent TemplateContent, NavContent NavContent,
    string OutputDirectory, string PageTitle)
{
    public GeneratedOutput GenerateIndividualBlogEntry(BlogEntry blogEntry)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(TemplateContent.Content);

        var title = PageTitle + " &mdash; " +
            blogEntry.Date.ToString("yyyy-MM-dd") + ": " +
            blogEntry.Title;

        var titleNode = doc.DocumentNode.Descendants("title").FirstOrDefault();
        if (titleNode == null)
        {
            throw new Exception("Could not find title node");
        }
        titleNode.InnerHtml = title;

        var h1Node = doc.DocumentNode.Descendants("h1").FirstOrDefault();
        if (h1Node == null)
        {
            throw new Exception("Expected h1 node to exist");
        }
        h1Node.InnerHtml = PageTitle;

        var navNode = doc.DocumentNode.Descendants("nav").FirstOrDefault();
        if (navNode == null)
        {
            throw new Exception("Expected nav node but did not find it");
        }
        navNode.InnerHtml = NavContent.Content;

        var mainNode = doc.DocumentNode.Descendants("main").FirstOrDefault();
        if (mainNode == null)
        {
            throw new Exception("Main node not found");
        }
        mainNode.InnerHtml = blogEntry.Content;

        string outputPath = Path.Combine(OutputDirectory,
        blogEntry.Date.Year.ToString(),
        blogEntry.Date.Month.ToString("D2"),
        blogEntry.Date.Day.ToString("D2"),
        "index.html");

        return new GeneratedOutput(outputPath, doc.DocumentNode.OuterHtml);
    }
}