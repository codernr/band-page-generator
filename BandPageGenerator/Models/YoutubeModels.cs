using System;

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

    public class YoutubeSnippetModel
    {
         public YoutubeVideoSnippetModel Snippet { get; set; }
    }

    public class YoutubeVideoSnippetModel
    {
        public DateTime PublishedAt { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public YoutubeThumbnailsCollectionModel Thumbnails { get; set; }

        public YoutubeResourceModel ResourceId { get; set; }
    }

    public class YoutubeThumbnailsCollectionModel
    {
        public YoutubeThumbnailModel Maxres { get; set; }
    }

    public class YoutubeThumbnailModel
    {
        public string Url { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
    }

    public class YoutubeResourceModel
    {
        public string VideoId { get; set; }
    }
}
