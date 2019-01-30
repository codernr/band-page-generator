namespace BandPageGenerator.Models
{
    public class SpotifyClientCredentialsModel
    {
        public string AccessToken { get; set; }

        public string TokenType { get; set; }

        public int ExpiresIn { get; set; }
    }

    public class SpotifyPagingModel<TModel>
    {
        public TModel[] Items { get; set; }
    }

    public class SpotifySimplifiedAlbumModel
    {
        public string Href { get; set; }
    }

    public class SpotifyAlbumModel
    {
        public string AlbumType { get; set; }

        public string Id { get; set; }

        public SpotifyImageModel[] Images { get; set; }

        public string Label { get; set; }

        public string Name { get; set; }

        public string ReleaseDate { get; set; }

        public string ReleaseDatePrecision { get; set; }

        public SpotifyPagingModel<SpotifyTrackModel> Tracks { get; set; }

        public string Type;
    }

    public class SpotifyImageModel
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public string Url { get; set; }
    }

    public class SpotifyTrackModel
    {
        public int DurationMs { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public int TrackNumber { get; set; }
    }
}
