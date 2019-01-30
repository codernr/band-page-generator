using BandPageGenerator.Config;
using BandPageGenerator.Services;
using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Stubble.Core.Builders;
using Stubble.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace BandPageGenerator
{
    public static class ServiceConfiguration
    {
        private static readonly Dictionary<string, Action<IServiceCollection, IConfigurationRoot>> modules =
            new Dictionary<string, Action<IServiceCollection, IConfigurationRoot>>
            {
                { "Facebook", ConfigureFacebook },
                { "Youtube", ConfigureYoutube },
                { "Spotify", ConfigureSpotify }
            };

        public static IServiceProvider ConfigureServiceProvider()
        {
            var serviceCollection = new ServiceCollection()
                .AddLogging(logging => logging.AddConsole())
                .AddSingleton<IAsyncStubbleRenderer>(new StubbleBuilder().Build())
                .AddSingleton<IViewRenderer, StubbleViewRenderer>();

            serviceCollection.AddHttpClient<IJsonHttpClient<SnakeCaseNamingStrategy>, JsonHttpClient<SnakeCaseNamingStrategy>>();
            serviceCollection.AddHttpClient<IJsonHttpClient<CamelCaseNamingStrategy>, JsonHttpClient<CamelCaseNamingStrategy>>();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();
            serviceCollection.AddOptions();

            foreach (var section in configuration.GetChildren()) modules[section.Key](serviceCollection, configuration);

            return serviceCollection.BuildServiceProvider();
        }

        private static void ConfigureFacebook(IServiceCollection serviceCollection, IConfigurationRoot configuration)
        {
            serviceCollection.Configure<Facebook>(configuration.GetSection("Facebook"));
            serviceCollection.AddSingleton<FacebookClient>();
        }

        private static void ConfigureYoutube(IServiceCollection serviceCollection, IConfigurationRoot configuration)
        {
            serviceCollection.Configure<Youtube>(configuration.GetSection("Youtube"));
            serviceCollection.AddSingleton<YoutubeClient>();
        }

        private static void ConfigureSpotify(IServiceCollection serviceCollection, IConfigurationRoot configuration)
        {
            serviceCollection.Configure<Spotify>(configuration.GetSection("Spotify"));
            serviceCollection.AddSingleton<SpotifyClient>();
        }
    }
}
