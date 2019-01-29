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

    /// <summary>
    /// Flattened model from API return values
    /// </summary>
    public class YoutubeVideoModel
    {
        public string Id { get; }

        public DateTime PublishedAt { get; }

        public string Title { get; }

        public string Description { get; }

        public YoutubeThumbnailModel Thumbnail { get; }

        public YoutubeVideoModel(YoutubeSnippetModel snippet)
        {
            this.Id = snippet.Snippet.ResourceId.VideoId;
            this.PublishedAt = snippet.Snippet.PublishedAt;
            this.Title = snippet.Snippet.Title;
            this.Description = snippet.Snippet.Description;
            this.Thumbnail = snippet.Snippet.Thumbnails.Maxres;
        }
    }
}
