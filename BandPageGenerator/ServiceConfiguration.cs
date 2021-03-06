﻿using BandPageGenerator.Config;
using BandPageGenerator.Services;
using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace BandPageGenerator
{
    public static class ServiceConfiguration
    {
        public static IServiceProvider ConfigureServiceProvider(string settings, string downloadSavePath)
        {
            var serviceCollection = new ServiceCollection()
                .AddSingleton<IViewRenderer, HandlebarsViewRenderer>()
                .AddSingleton<IFileSystem, FileSystem>();

            ConfigureLogging(serviceCollection);

            serviceCollection.AddHttpClient<IJsonHttpClient<SnakeCaseNamingStrategy>, JsonHttpClient<SnakeCaseNamingStrategy>>();
            serviceCollection.AddHttpClient<IJsonHttpClient<CamelCaseNamingStrategy>, JsonHttpClient<CamelCaseNamingStrategy>>();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), settings), false)
                .Build();
            serviceCollection.AddOptions();

            ConfigureDownloaderService(serviceCollection, downloadSavePath, configuration);

            AddProviderServices<FacebookConfig, IFacebookClient, FacebookClient, FacebookTemplateDataTransformer>(
                "Facebook", configuration, serviceCollection);
            AddProviderServices<YoutubeConfig, IYoutubeClient, YoutubeClient, YoutubeTemplateDataTransformer>(
                "Youtube", configuration, serviceCollection);
            AddProviderServices<SpotifyConfig, ISpotifyClient, SpotifyClient, SpotifyTemplateDataTransformer>(
                "Spotify", configuration, serviceCollection);

            return serviceCollection.BuildServiceProvider();
        }

        private static void ConfigureLogging(IServiceCollection serviceCollection)
        {
            serviceCollection.AddLogging(logging => logging
                .AddConsole(options => options.IncludeScopes = true)
                .AddFilter("System.Net.Http.HttpClient", LogLevel.Error));
        }

        private static void AddProviderServices<TConfig, IClient, TClient, TTemplateDataTransformer>
            (string key, IConfigurationRoot configuration, IServiceCollection collection)
            where TConfig : class where TClient : class, IClient where TTemplateDataTransformer : class, ITemplateDataTransformer
            where IClient : class
        {
            var section = configuration.GetChildren().FirstOrDefault(s => s.Key == key);

            if (section == null)
            {
                return;
            }

            collection.Configure<TConfig>(section);
            collection.AddSingleton<IClient, TClient>();
            collection.AddSingleton<ITemplateDataTransformer, TTemplateDataTransformer>();
        }

        private static void ConfigureDownloaderService(
            IServiceCollection serviceCollection, string downloadSavePath, IConfigurationRoot configuration)
        {
            var configSection = configuration.GetSection("General");
            var downloadedBasePath = configSection["DownloadedBasePath"];

            if (downloadSavePath == null || configSection == null || downloadedBasePath == null)
            {
                serviceCollection.AddSingleton<IDownloaderClient, NullDownloaderClient>();
                return;
            }

            serviceCollection.Configure<GeneralConfig>(generalConfig => {
                generalConfig.DownloadedBasePath = downloadedBasePath;
                generalConfig.DownloadSavePath = downloadSavePath;
            });

            serviceCollection.AddHttpClient<IDownloaderClient, DownloaderClient>();
        }
    }
}
