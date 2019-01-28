namespace BandPageGenerator.Models
{
    public class YoutubeDataWrapperModel<TModel>
    {
        public TModel[] Items { get; set; }
    }

    public class YoutubeDataModel
    {
        public YoutubeStatisticsModel Statistics { get; set; }
    }

    public class YoutubeStatisticsModel
    {
        public long ViewCount { get; set; }
    }
}
