﻿using BandPageGenerator.Config;
using BandPageGenerator.Services;
using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Linq;

namespace BandPageGenerator
{
    public static class ServiceConfiguration
    {
        public static IServiceProvider ConfigureServiceProvider(string settings, string downloadSavePath)
        {
            var serviceCollection = new ServiceCollection()
                .AddLogging(logging => logging.AddConsole())
                .AddSingleton<IViewRenderer, HandlebarsViewRenderer>();

            serviceCollection.AddHttpClient<IJsonHttpClient<SnakeCaseNamingStrategy>, JsonHttpClient<SnakeCaseNamingStrategy>>();
            serviceCollection.AddHttpClient<IJsonHttpClient<CamelCaseNamingStrategy>, JsonHttpClient<CamelCaseNamingStrategy>>();
            serviceCollection.AddHttpClient<DownloaderClient>();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), settings), false)
                .Build();
            serviceCollection.AddOptions();

            serviceCollection.Configure<GeneralConfig>(generalConfig => {
                generalConfig.DownloadedBasePath = configuration.GetSection("GeneralConfig")["DownloadedBasePath"];
                generalConfig.DownloadSavePath = downloadSavePath;
            });

            AddProviderServices<FacebookConfig, FacebookClient, FacebookTemplateDataTransformer>(
                "Facebook", configuration, serviceCollection);
            AddProviderServices<YoutubeConfig, YoutubeClient, YoutubeTemplateDataTransformer>(
                "Youtube", configuration, serviceCollection);
            AddProviderServices<SpotifyConfig, SpotifyClient, SpotifyTemplateDataTransformer>(
                "Spotify", configuration, serviceCollection);

            return serviceCollection.BuildServiceProvider();
        }

        private static void AddProviderServices<TConfig, TClient, TTemplateDataTransformer>
            (string key, IConfigurationRoot configuration, IServiceCollection collection)
            where TConfig : class where TClient : class where TTemplateDataTransformer : class, ITemplateDataTransformer
        {
            var section = configuration.GetChildren().FirstOrDefault(s => s.Key == key);

            if (section == null) return;

            collection.Configure<TConfig>(section);
            collection.AddSingleton<TClient>();
            collection.AddSingleton<ITemplateDataTransformer, TTemplateDataTransformer>();
        }
    }
}
