namespace StaticSebugGenerator;

public record BlogEntry(DateOnly Date, string Title,
    string Content,
    string FilePath)
{

}