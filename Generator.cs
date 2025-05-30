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
        var outputGenerator = new OutputGenerator(templateContent, navContent,
            Settings.TargetDirectory, Settings.PageTitle);

        await foreach (var entry in entries)
        {
            readBlogEntries.Add(entry);
            var generatedOutput = outputGenerator.GenerateIndividualBlogEntry(entry);
            generatedOutputs.Add(generatedOutput);
        }

        var generatedOutputWriter = new GeneratedOutputWriter(Settings.TargetDirectory);
        foreach (var generatedOutput in generatedOutputs)
        {
            var (outputAfter, didChange) = await generatedOutputWriter.Write(generatedOutput);
            if (didChange)
            {
                Console.WriteLine(outputAfter.FilePath);
            }
        }
    }
}