using System.Text;

namespace StaticSebugGenerator;

public record TemplateContent(string Path, string Content)
{
    public static async Task<TemplateContent> GetFromSourceDirectory(string sourceDirectory)
    {
        string templatePath = System.IO.Path.Combine(sourceDirectory, "template.html");
        string templateContent = await File.ReadAllTextAsync(templatePath, Encoding.UTF8);
        return new TemplateContent(templatePath, templateContent);
    }
}