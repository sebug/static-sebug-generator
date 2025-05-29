using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
.AddUserSecrets<Program>()
.Build();

var settings = config.GetRequiredSection("StaticSebugGenerator")
    .Get<StaticSebugGeneratorOptions>() ??
    throw new Exception("Could not get settings for static sebug generator");

Console.WriteLine(settings.SourceDirectory);

