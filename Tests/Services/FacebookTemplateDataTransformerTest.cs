using BandPageGenerator.Config;
using BandPageGenerator.Models;
using BandPageGenerator.Services;
using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Services
{
    public static class FacebookTemplateDataTransformerTest
    {
        [Fact]
        public async static Task ShouldAddTemplateDataKeys()
        {
            var futureStartTime = DateTime.Now + TimeSpan.FromDays(1);
            var futureEndTime = DateTime.Now + TimeSpan.FromDays(2);

            var clientMock = new Mock<IFacebookClient>();
            clientMock.Setup(c => c.GetPageLikeCountAsync()).ReturnsAsync(2134);
            clientMock.Setup(c => c.GetPageEventsAsync()).ReturnsAsync(new List<FacebookEventModel>
            {
                new FacebookEventModel
                {
                    Id = "1234",
                    Category = "Music",
                    Cover = new FacebookCoverPhotoModel
                    {
                        Id = "photo0",
                        OffsetX = 1,
                        OffsetY = 1,
                        Source = "http://example.com/1"
                    },
                    Description = "hello",
                    EndTime = futureEndTime,
                    Name = "Test event",
                    Place = new FacebookPlaceModel
                    {
                        Id = "place0",
                        Name = "place 0",
                        Location = new FacebookLocationModel
                        {
                            City = "TestCity",
                            Country = "TestCountry",
                            Latitude = 1.2f,
                            Longitude = 2.3f,
                            Name = "TestLocationName",
                            State = "TestState",
                            Street = "TestStreet",
                            Zip = "1111"
                        }
                    },
                    StartTime = futureStartTime,
                    TicketUri = "http://ticket.com"
                },
                new FacebookEventModel
                {
                    Id = "PastEvent",
                    StartTime = DateTime.Now - TimeSpan.FromDays(2),
                    EndTime = DateTime.Now - TimeSpan.FromDays(1),
                    Cover = new FacebookCoverPhotoModel
                    {
                        Id = "pastphoto0",
                        OffsetX = 1,
                        OffsetY = 1,
                        Source = "http://example.com/xcv"
                    },
                }
            });

            clientMock.Setup(c => c.GetProfilePictureAsync()).ReturnsAsync("http://example.com/2");
            clientMock.Setup(c => c.GetRecentInstagramPhotosAsync()).ReturnsAsync(new[]
            {
                new FacebookInstagramMediaModel
                {
                    MediaType = "IMAGE",
                    MediaUrl = "http://example.com/3",
                    Caption = "Caption",
                    Permalink = "http://permalink.com",
                    Timestamp = new DateTime(2019, 3, 28),
                    Id = "InstaId0"
                }
            });

            clientMock.Setup(c => c.GetAlbumAsync("featured")).ReturnsAsync(new List<FacebookAlbumPhotosModel>
            {
                new FacebookAlbumPhotosModel
                {
                    Id = "Featured0",
                    Name = "FeaturedPhotos",
                    Images = new[]
                    {
                        new FacebookPhotoModel { Id = "FeaturedPhoto0", Height = 20, Width = 10, Source = "http://example.com/4" }
                    }
                }
            });

            clientMock.Setup(c => c.GetAlbumAsync("members")).ReturnsAsync(new List<FacebookAlbumPhotosModel>
            {
                new FacebookAlbumPhotosModel
                {
                    Id = "Members0",
                    Name = "Jack" + '\n' + "Guitars",
                    Images = new[]
                    {
                        new FacebookMemberPhotoModel { Id = "member0", Height = 34, Width = 56, Source = "http://example.com/5" }
                    }
                }
            });

            var downloaderMock = new Mock<IDownloaderClient>();
            downloaderMock.Setup(d => d.DownloadFile("http://example.com/1", "photo0", It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("example1");
            downloaderMock.Setup(d => d.DownloadFile("http://example.com/2", "profile", It.IsAny<string>(), It.IsAny<string>(), true))
                .ReturnsAsync("example2");
            downloaderMock.Setup(d => d.DownloadFile("http://example.com/3", "InstaId0", It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("example3");
            downloaderMock.Setup(d => d.DownloadFile("http://example.com/4", "Featured0", It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("example4");
            downloaderMock.Setup(d => d.DownloadFile("http://example.com/5", "Members0", It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("example5");

            var optionsMock = new Mock<IOptions<GeneralConfig>>();
            optionsMock.Setup(o => o.Value).Returns(new GeneralConfig());

            var facebookOptionsMock = new Mock<IOptions<FacebookConfig>>();
            facebookOptionsMock.Setup(o => o.Value).Returns(new FacebookConfig
            {
                PastEventDisplayLimit = 5,
                AlbumId = "featured",
                MembersAlbumId = "members"
            });

            var transformer = new FacebookTemplateDataTransformer(clientMock.Object, facebookOptionsMock.Object, downloaderMock.Object, optionsMock.Object);

            var data = new Dictionary<string, object>();

            await transformer.AddTemplateDataAsync(data);

            Assert.True(data.ContainsKey("Likes"));
            Assert.True(data.ContainsKey("ProfilePictureUrl"));
            Assert.True(data.ContainsKey("UpcomingEvents"));
            Assert.True(data.ContainsKey("PastEvents"));
            Assert.True(data.ContainsKey("FeaturedPhotos"));
            Assert.True(data.ContainsKey("MemberPhotos"));
            Assert.True(data.ContainsKey("InstagramPhotos"));

            Assert.Equal(2134, (int)data["Likes"]);
            Assert.Equal("example2", data["ProfilePictureUrl"]);

            EventsAssertions((data["UpcomingEvents"] as IEnumerable<FacebookEventModel>).ToArray()[0], futureStartTime, futureEndTime);
            Assert.Equal("PastEvent", (data["PastEvents"] as IEnumerable<FacebookEventModel>).ToArray()[0].Id);

            FeaturedPhotosAssertions((data["FeaturedPhotos"] as IEnumerable<FacebookPhotoModel>).ToArray()[0]);

            MemberPhotosAssertions((data["MemberPhotos"] as IEnumerable<FacebookMemberPhotoModel>).ToArray()[0]);

            InstagramPhotosAssertions((data["InstagramPhotos"] as IEnumerable<FacebookInstagramMediaModel>).ToArray()[0]);
        }

        private static void EventsAssertions(FacebookEventModel model, DateTime futureStartTime, DateTime futureEndTime)
        {
            Assert.Equal("1234", model.Id);
            Assert.Equal("Music", model.Category);
            Assert.Equal("photo0", model.Cover.Id);
            Assert.Equal(1, model.Cover.OffsetX);
            Assert.Equal(1, model.Cover.OffsetY);
            Assert.Equal("example1", model.Cover.Source);
            Assert.Equal("hello", model.Description);
            Assert.Equal(futureEndTime, model.EndTime);
            Assert.Equal("Test event", model.Name);
            Assert.Equal("place0", model.Place.Id);
            Assert.Equal("place 0", model.Place.Name);
            Assert.Equal("TestCity", model.Place.Location.City);
            Assert.Equal("TestCountry", model.Place.Location.Country);
            Assert.Equal(1.2f, model.Place.Location.Latitude);
            Assert.Equal(2.3f, model.Place.Location.Longitude);
            Assert.Equal("TestLocationName", model.Place.Location.Name);
            Assert.Equal("TestState", model.Place.Location.State);
            Assert.Equal("TestStreet", model.Place.Location.Street);
            Assert.Equal("1111", model.Place.Location.Zip);
            Assert.Equal(futureStartTime, model.StartTime);
            Assert.Equal("http://ticket.com", model.TicketUri);
        }

        private static void FeaturedPhotosAssertions(FacebookPhotoModel model)
        {
            Assert.Equal("Featured0", model.Id);
            Assert.Equal(20, model.Height);
            Assert.Equal(10, model.Width);
            Assert.Equal("example4", model.Source);
        }

        private static void MemberPhotosAssertions(FacebookMemberPhotoModel model)
        {
            Assert.Equal("Members0", model.Id);
            Assert.Equal(34, model.Height);
            Assert.Equal(56, model.Width);
            Assert.Equal("example5", model.Source);
            Assert.Equal("Jack", model.Name);
            Assert.Equal("Guitars", model.Description);
        }

        private static void InstagramPhotosAssertions(FacebookInstagramMediaModel model)
        {
            Assert.Equal("IMAGE", model.MediaType);
            Assert.Equal("example3", model.MediaUrl);
            Assert.Equal("Caption", model.Caption);
            Assert.Equal("http://permalink.com", model.Permalink);
            Assert.Equal(new DateTime(2019, 03, 28), model.Timestamp);
            Assert.Equal("InstaId0", model.Id);
        }
    }
}
