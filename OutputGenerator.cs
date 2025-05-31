using System.Text;
using HtmlAgilityPack;

namespace StaticSebugGenerator;

public record OutputGenerator(TemplateContent TemplateContent, NavContent NavContent,
    string OutputDirectory, string PageTitle)
{
    public GeneratedOutput GenerateIndividualBlogEntry(BlogEntry blogEntry,
        BlogEntry? previousEntry,
        BlogEntry? nextEntry)
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
        h1Node.InnerHtml = "<a href=\"/\">" + PageTitle + "</a>";

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


        var sb = new StringBuilder();
        sb.Append("<p class=\"entries-navigation\">");
        if (previousEntry != null)
        {
            sb.AppendLine("<a rel=\"prev\" href=\"/" + previousEntry.Date.ToString("yyyy-MM-dd").Replace("-", "/") +
                "/\">Previous: " + previousEntry.Title + "</a>");
            if (nextEntry != null)
            {
                sb.AppendLine(" / ");
            }
        }
        if (nextEntry != null)
        {
            sb.AppendLine("<a rel=\"next\" href=\"/" + nextEntry.Date.ToString("yyyy-MM-dd").Replace("-", "/") +
                "/\">Next: " + nextEntry.Title + "</a>");
        }
        sb.AppendLine("<a href=\"/\">Latest Entries</a>");
        sb.Append("</p>");
        mainNode.InnerHtml += sb.ToString();

        string outputPath = Path.Combine(OutputDirectory,
        blogEntry.Date.Year.ToString(),
        blogEntry.Date.Month.ToString("D2"),
        blogEntry.Date.Day.ToString("D2"),
        "index.html");

        return new GeneratedOutput(outputPath, doc.DocumentNode.OuterHtml);
    }

    public GeneratedOutput GenerateLinearGroupedPage(int pageNumber, List<BlogEntry> entries, bool hasOlder,
        bool hasNewer)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(TemplateContent.Content);

        var title = PageTitle + (pageNumber > 1 ? (" &mdash; Page " + pageNumber) : String.Empty);

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
        h1Node.InnerHtml = "<a href=\"/\">" + PageTitle + "</a>";

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

        mainNode.InnerHtml = String.Join(Environment.NewLine, entries
            .OrderByDescending(entry => entry.Date)
            .Select(entry => entry.Content));

        var h2s = mainNode.Descendants("h2").ToList();
        foreach (var h2 in h2s)
        {
            var correspodingBlogEntry =
                entries.FirstOrDefault(entry => h2.InnerHtml.Contains(
                    entry.Date.ToString("yyyy-MM-dd")));

            if (correspodingBlogEntry != null)
            {
                h2.InnerHtml = "<a href=\"/" + correspodingBlogEntry.Date
                    .ToString("yyyy-MM-dd").Replace("-", "/") + "/\">" +
                    h2.InnerHtml + "</a>";
            }
        }

        var sb = new StringBuilder();
        sb.Append("<p class=\"entries-navigation\">");
        if (hasOlder)
        {
            sb.AppendLine("<a rel=\"prev\" href=\"/" + (pageNumber + 1) +
                "/\">Older Entries</a>");
            if (hasNewer)
            {
                sb.AppendLine(" / ");
            }
        }
        if (hasNewer)
        {
            if (pageNumber > 2)
            {
                sb.AppendLine("<a rel=\"next\" href=\"/" + (pageNumber - 1) +
                "/\">Newer Entries</a>");
            }
            sb.AppendLine("<a href=\"/\">Latest Entries</a>");
        }
        sb.Append("</p>");
        mainNode.InnerHtml += sb.ToString();

        string outputPath;
        if (pageNumber == 1)
        {
            outputPath = Path.Combine(OutputDirectory, "index.html");
        }
        else
        {
            outputPath = Path.Combine(OutputDirectory, pageNumber.ToString(), "index.html");
        }

        return new GeneratedOutput(outputPath, doc.DocumentNode.OuterHtml);
    }
}