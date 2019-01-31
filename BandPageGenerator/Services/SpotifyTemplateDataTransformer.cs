using BandPageGenerator.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BandPageGenerator.Services
{
    public class SpotifyTemplateDataTransformer : ITemplateDataTransformer
    {
        private readonly SpotifyClient client;

        public SpotifyTemplateDataTransformer(SpotifyClient client) => this.client = client;

        public async Task AddTemplateData(Dictionary<string, object> templateData)
        {
            templateData.Add("Albums", await this.client.GetAlbumsAsync());
        }
}
}
