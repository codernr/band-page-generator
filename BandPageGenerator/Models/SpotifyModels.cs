﻿using System;
using System.Collections.Generic;

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

    public class SpotifyAlbumBaseModel
    {
        public string AlbumType { get; set; }

        public string Id { get; set; }

        public string Label { get; set; }

        public string Name { get; set; }
        
        public string Type { get; set; }
    }

    public class SpotifyAlbumModel : SpotifyAlbumBaseModel
    {
        public string ReleaseDatePrecision { get; set; }

        public string ReleaseDate { get; set; }

        public SpotifyImageModel[] Images { get; set; }

        public SpotifyPagingModel<SpotifyTrackModel> Tracks { get; set; }
    }

    public class SpotifyAlbumTemplateModel : SpotifyAlbumBaseModel
    {
        public SpotifyImageModel Image { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public SpotifyTrackModel[] Tracks { get; set; }

        public Dictionary<string, string> AlternativeLinks { get; set; }
    }

    public class SpotifyImageModel
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public string Url { get; set; }
    }

    public class SpotifyTrackModel
    {
        public double DurationMs { get; set; }

        public TimeSpan Duration
        {
            get
            {
                return TimeSpan.FromMilliseconds(this.DurationMs);
            }
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public int TrackNumber { get; set; }
    }
}
