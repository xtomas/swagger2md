using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        var switchMappings = new Dictionary<string, string>()
        {
            { "-i", "inputFile" },
            { "-st", "skipTitle" },
            { "-o", "outputFile" },
            { "-sp", "subPages" },
        };

        config.AddCommandLine(args, switchMappings);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddTransient<DocumentGenerator>();
    }).Build();

using var scope = host.Services.CreateScope();
var services = scope.ServiceProvider;

services.GetRequiredService<DocumentGenerator>().GenerateDocument();