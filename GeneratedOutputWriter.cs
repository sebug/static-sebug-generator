using System.Text;

namespace StaticSebugGenerator;

public record GeneratedOutputWriter(string TargetDirectory)
{
    public async Task<(GeneratedOutput generatedOutput, bool didChange)> Write(GeneratedOutput output)
    {
        string directoryName = Path.GetDirectoryName(output.FilePath) ??
            throw new Exception("Expected to have been able to get directory from file path " + output.FilePath);
        Directory.CreateDirectory(directoryName);
        if (File.Exists(output.FilePath))
        {
            string contentBefore = await File.ReadAllTextAsync(output.FilePath, Encoding.UTF8);
            if (contentBefore == output.Content)
            {
                return (output, false);
            }
        }
        await File.WriteAllTextAsync(output.FilePath, output.Content);
        return (output, true);
    }
}