using BandPageGenerator.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BandPageGenerator.Services
{
    public class FacebookGraph
    {
        private readonly Facebook config;
        private readonly ILogger logger;

        public FacebookGraph(IOptions<Facebook> config, ILoggerFactory loggerFactory)
        {
            this.config = config.Value;
            this.logger = loggerFactory.CreateLogger<FacebookGraph>();
        }
    }
}
