using Microsoft.Extensions.Configuration;
using StaticSebugGenerator;

var config = new ConfigurationBuilder()
.AddUserSecrets<Program>()
.Build();

var settings = config.GetRequiredSection("StaticSebugGenerator")
    .Get<StaticSebugGeneratorOptions>() ??
    throw new Exception("Could not get settings for static sebug generator");

var generator = new Generator(settings);

await generator.Run();