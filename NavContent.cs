using System.Text;

namespace StaticSebugGenerator;

public record NavContent(string Path, string Content)
{
    public static async Task<NavContent> GetFromSourceDirectory(string sourceDirectory)
    {
        string linkbarPath = System.IO.Path.Combine(sourceDirectory, "linkbar.html");
        string linkbarContent = await File.ReadAllTextAsync(linkbarPath, Encoding.UTF8);
        return new NavContent(linkbarPath, linkbarContent);
    }
}