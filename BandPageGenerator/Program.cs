using BandPageGenerator.Config;
using BandPageGenerator.Services;
using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Stubble.Core.Builders;
using Stubble.Core.Interfaces;
using System;
using System.IO;

namespace BandPageGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = ConfigureServices().BuildServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();

            var renderer = serviceProvider.GetService<IViewRenderer>();

            var model = new { Name = "Anybody" };

            var renderTask = renderer.RenderViewToStringAsync<object>(
                Path.Combine(Directory.GetCurrentDirectory(), "Templates/index.html"),
                model);

            renderTask.Wait();

            logger.LogInformation(renderTask.Result);

            var fbTask = serviceProvider.GetService<FacebookGraph>().GetPageLikeCountAsync();
            fbTask.Wait();
            logger.LogInformation("Count: " + fbTask.Result);

            Console.ReadKey();
        }

        static IServiceCollection ConfigureServices()
        {
            var serviceCollection = new ServiceCollection()
                .AddLogging(logging => logging.AddConsole())
                .AddSingleton<IAsyncStubbleRenderer>(new StubbleBuilder().Build())
                .AddSingleton<IViewRenderer, StubbleViewRenderer>()
                .AddSingleton(new JsonSerializer
                {
                    ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() }
                });

            serviceCollection.AddHttpClient<IFormattedHttpClient, JsonHttpClient>();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();
            serviceCollection.AddOptions();
            serviceCollection.Configure<Facebook>(configuration.GetSection("Facebook"));
            serviceCollection.Configure<Youtube>(configuration.GetSection("Youtube"));

            serviceCollection.AddSingleton<FacebookGraph>();

            return serviceCollection;
        }
    }
}
