﻿namespace BandPageGenerator.Config
{
    public class FacebookConfig
    {
        public string PageId { get; set; }

        public string InstagramId { get; set; }

        public string[] FilterHashtags { get; set; }

        public string AlbumId { get; set; }

        public string MembersAlbumId { get; set; }

        public string ApiVersion { get; set; }

        public string AccessToken { get; set; }

        public int PastEventDisplayLimit { get; set; }
    }
}
