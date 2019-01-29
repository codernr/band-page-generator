﻿using BandPageGenerator.Config;
using BandPageGenerator.Models;
using BandPageGenerator.Services;
using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json.Serialization;
using System;
using Xunit;

namespace Tests
{
    public class YoutubeClientTest
    {
        [Fact]
        public async void ShouldTranslateToFlattenedItemCorrectly()
        {
            var client = new Mock<IJsonHttpClient<CamelCaseNamingStrategy>>();

            client.Setup(c => c.GetAsync<YoutubeDataWrapperModel<YoutubeSnippetModel>>(It.IsAny<string>()))
                .ReturnsAsync(new YoutubeDataWrapperModel<YoutubeSnippetModel>
                {
                    Items = new[] { new YoutubeSnippetModel { Snippet = new YoutubeVideoSnippetModel
                    {
                        Title = "title", Description = "description", PublishedAt = new DateTime(2000, 10, 10),
                        ResourceId = new YoutubeResourceModel { VideoId = "abcd" },
                        Thumbnails = new YoutubeThumbnailsCollectionModel { Maxres = new YoutubeThumbnailModel
                        {
                            Width = 1, Height = 2, Url = "xyz"
                        }
                        }
                    } } }
                });

            var options = new Mock<IOptions<Youtube>>();
            options.Setup(o => o.Value).Returns(new Youtube());

            var youtubeClient = new YoutubeClient(options.Object, client.Object);

            var data = await youtubeClient.GetFeaturedVideos();

            Assert.Single(data);
            Assert.Equal("title", data[0].Title);
            Assert.Equal("description", data[0].Description);
            Assert.Equal(new DateTime(2000, 10, 10), data[0].PublishedAt);
            Assert.Equal("abcd", data[0].Id);
            Assert.Equal(1, data[0].Thumbnail.Width);
            Assert.Equal(2, data[0].Thumbnail.Height);
            Assert.Equal("xyz", data[0].Thumbnail.Url);
        }
    }
}