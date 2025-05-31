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

        // At the moment, we have to evaluate all of them to make the back / forward links
        // We may be more clever about that in the future
        await foreach (var entry in entries)
        {
            readBlogEntries.Add(entry);
        }

        for (int i = 0; i < readBlogEntries.Count; i += 1)
        {
            var entry = readBlogEntries[i];
            BlogEntry? previousEntry;
            if (i > 0)
            {
                previousEntry = readBlogEntries[i - 1];
            }
            else
            {
                previousEntry = null;
            }
            BlogEntry? nextEntry;
            if (i < readBlogEntries.Count - 1)
            {
                nextEntry = readBlogEntries[i + 1];
            }
            else
            {
                nextEntry = null;
            }
            var generatedOutput = outputGenerator.GenerateIndividualBlogEntry(entry,
                previousEntry, nextEntry);
            generatedOutputs.Add(generatedOutput);
        }

        // Now, create the linear archive - 5 entries per pag
        var descendingBlogEntries = readBlogEntries.OrderByDescending(entry => entry.Date).ToList();
        int entriesPerPage = 5;
        int pageNumber = 1;
        for (int i = 0; i < descendingBlogEntries.Count; i += entriesPerPage, pageNumber += 1)
        {
            var linearEntries = descendingBlogEntries.Skip(i).Take(entriesPerPage).ToList();
            bool hasOlderPage = i + entriesPerPage < descendingBlogEntries.Count;
            bool hasNewerPage = descendingBlogEntries.Count > entriesPerPage && i > 0;
            var generatedOutput = outputGenerator.GenerateLinearGroupedPage(pageNumber,
            linearEntries, hasOlderPage, hasNewerPage);
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