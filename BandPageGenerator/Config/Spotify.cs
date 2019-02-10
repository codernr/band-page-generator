using System.Collections.Generic;

namespace BandPageGenerator.Config
{
    public class Spotify
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string ArtistId { get; set; }

        public AlternativeLink[] AlternativeLinks { get; set; }
    }

    public class AlternativeLink
    {
        public string Title { get; set; }

        public Dictionary<string, string> Links { get; set; }
    }
}
