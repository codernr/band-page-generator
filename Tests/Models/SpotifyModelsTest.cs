﻿using BandPageGenerator.Models;
using System;
using Xunit;

namespace Tests.Models
{
    public class SpotifyModelsTest
    {
        [Fact]
        public static void ShouldConvertMillisecondsToTimeSpan()
        {
            var model = new SpotifyTrackModel
            {
                DurationMs = 61000
            };

            Assert.Equal(TimeSpan.FromSeconds(61), model.Duration);
        }
    }
}
