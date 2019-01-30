namespace BandPageGenerator.Models
{
    public class SpotifyClientCredentialsModel
    {
        public string AccessToken { get; set; }

        public string TokenType { get; set; }

        public int ExpiresIn { get; set; }
    }
}
