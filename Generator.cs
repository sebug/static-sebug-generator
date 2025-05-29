namespace StaticSebugGenerator;

public record Generator(StaticSebugGeneratorOptions Settings)
{
    public async Task Run()
    {
        var entriesReader = new BlogEntriesReader();
        var entries = entriesReader.GetBlogEntries(Settings.SourceDirectory);
        List<BlogEntry> readBlogEntries = [];
        List<GeneratedOutput> generatedOutputs = [];

        var templateContent = await TemplateContent.GetFromSourceDirectory(Settings.SourceDirectory);
        var navContent = await NavContent.GetFromSourceDirectory(Settings.SourceDirectory);
        var outputGenerator = new OutputGenerator(templateContent, navContent);

        await foreach (var entry in entries)
        {
            readBlogEntries.Add(entry);
            Console.WriteLine(entry.Date.ToString() + " - " + entry.Title);
        }
    }
}