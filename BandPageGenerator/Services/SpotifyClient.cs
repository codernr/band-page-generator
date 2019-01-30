﻿using BandPageGenerator.Config;
using BandPageGenerator.Models;
using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BandPageGenerator.Services
{
    public class SpotifyClient
    {
        private readonly Spotify config;
        private readonly IFormattedHttpClient client;
        private const string apiUri = "https://api.spotify.com/v1/";
        private const string authUri = "https://accounts.spotify.com/api/token";
        private SpotifyClientCredentialsModel credentials;

        public SpotifyClient(IOptions<Spotify> config, IJsonHttpClient<SnakeCaseNamingStrategy> client)
        {
            this.config = config.Value;
            this.client = client;
        }

        private async Task<SpotifyClientCredentialsModel> GetCredentialsAsync()
        {
            if (this.credentials != null) return this.credentials;

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

            var key = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{this.config.ClientId}:{this.config.ClientSecret}"));

            this.credentials = await this.client.PostAsync<SpotifyClientCredentialsModel>(
                authUri, content, new[] { ("Authorization", $"Basic {key}") });

            return this.credentials;
        }

        private async Task<TModel> GetApiDataAsync<TModel>(string edge, params (string, string)[] parameters)
        {
            var credentials = await this.GetCredentialsAsync();

            var headers = new[] { ("Authorization", $"Bearer {credentials.AccessToken}") };

            return await this.client.GetAsync<TModel>(apiUri + edge, headers);
        }
    }
}
