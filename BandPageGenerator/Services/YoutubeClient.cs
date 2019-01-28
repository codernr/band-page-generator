using BandPageGenerator.Config;
using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace BandPageGenerator.Services
{
    public class YoutubeClient
    {
        private readonly Youtube config;
        private readonly IFormattedHttpClient client;

        public YoutubeClient(IOptions<Youtube> config, IFormattedHttpClient client)
        {
            this.config = config.Value;
            this.client = client;
        }
    }
}
