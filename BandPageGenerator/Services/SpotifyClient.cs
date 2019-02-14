using BandPageGenerator.Config;
using BandPageGenerator.Models;
using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BandPageGenerator.Services
{
    public class SpotifyClient
    {
        private readonly SpotifyConfig config;
        private readonly IFormattedHttpClient client;
        private const string apiUri = "https://api.spotify.com/v1/";
        private const string authUri = "https://accounts.spotify.com/api/token";
        private SpotifyClientCredentialsModel credentials;

        public SpotifyClient(IOptions<SpotifyConfig> config, IJsonHttpClient<SnakeCaseNamingStrategy> client)
        {
            this.config = config.Value;
            this.client = client;
        }

        public async Task<SpotifyAlbumModel[]> GetAlbumsAsync()
        {
            var simpleAlbumsData = await this.GetPagedApiDataAsync<SpotifySimplifiedAlbumModel>(
                $"artists/{this.config.ArtistId}/albums", ("include_groups", "album,single"));

            var albumTasks = simpleAlbumsData.Select(d => this.GetAuthorizedUriAsync<SpotifyAlbumModel>(d.Href));

            await Task.WhenAll(albumTasks);

            return albumTasks.Select(t => t.Result).ToArray();
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

        private async Task<TModel[]> GetPagedApiDataAsync<TModel>(string edge, params (string, string)[] parameters)
        {
            var pagedData = await this.GetApiDataAsync<SpotifyPagingModel<TModel>>(edge, parameters);

            return pagedData.Items;
        }

        private Task<TModel> GetApiDataAsync<TModel>(string edge, params (string, string)[] parameters)
        {
            var requestUri = apiUri + edge;

            if (parameters.Length > 0)
            {
                requestUri += "?" + string.Join("&", parameters.Select(p => $"{p.Item1}={p.Item2}"));
            }

            return this.GetAuthorizedUriAsync<TModel>(requestUri);
        }

        private async Task<TModel> GetAuthorizedUriAsync<TModel>(string requestUri)
        {
            var credentials = await this.GetCredentialsAsync();

            var headers = new[] { ("Authorization", $"Bearer {credentials.AccessToken}") };

            return await this.client.GetAsync<TModel>(requestUri, headers);
        }
    }
}
