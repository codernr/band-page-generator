using System;

namespace BandPageGenerator.Models
{
    public class FacebookFanCountModel
    {
        public int FanCount { get; set; }
    }

    /// <summary>
    /// Event model: https://developers.facebook.com/docs/graph-api/reference/event/
    /// </summary>
    public class FacebookEventModel
    {
        public string Id { get; set; }

        public int AttendingCount { get; set; }

        public string Category { get; set; }

        public FacebookCoverPhotoModel Cover { get; set; }

        public string Description { get; set; }

        public DateTime EndTime { get; set; }

        public string Name { get; set; }
    }

    /// <summary>
    /// Cover photo model: https://developers.facebook.com/docs/graph-api/reference/cover-photo/
    /// </summary>
    public class FacebookCoverPhotoModel
    {
        public string Id { get; set; }

        public float OffsetX { get; set; }

        public float OffsetY { get; set; }

        public string Source { get; set; }
    }

    /// <summary>
    /// Place model: https://developers.facebook.com/docs/graph-api/reference/place/
    /// </summary>
    public class FacebookPlaceModel
    {
        public string Id { get; set; }

        public FacebookLocationModel Location { get; set; }

        public string Name { get; set; }
    }

    /// <summary>
    /// Location model: https://developers.facebook.com/docs/graph-api/reference/location/
    /// </summary>
    public class FacebookLocationModel
    {
        public string City { get; set; }

        public string Country { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }

        public string Name { get; set; }

        public string State { get; set; }

        public string Street { get; set; }

        public string Zip { get; set; }
    }
}
