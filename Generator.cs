namespace StaticSebugGenerator;

public record Generator(StaticSebugGeneratorOptions Settings)
{
    public async Task Run()
    {
        var entriesReader = new BlogEntriesReader();
        var entries = entriesReader.GetBlogEntries(Settings.SourceDirectory);
        await foreach (var entry in entries)
        {
            Console.WriteLine(entry.Date.ToString() + " - " + entry.Title);
        }
    }
}