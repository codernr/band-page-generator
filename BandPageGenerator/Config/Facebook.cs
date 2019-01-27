namespace BandPageGenerator.Config
{
    public class Facebook
    {
        public string AppId { get; set; }

        public string AppSecret { get; set; }

        public string PageId { get; set; }

        public string AlbumId { get; set; }

        public string AccessToken => $"{this.AppId}|{this.AppSecret}";
    }
}
