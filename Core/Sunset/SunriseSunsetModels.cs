namespace Lights.Core
{
    using System;
    using System.Text.Json.Serialization;

    public partial class SunriseSunsetResult
    {
        [JsonPropertyName("results")]
        public Results Results { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }

    public partial class Results
    {
        [JsonPropertyName("sunrise")]
        public string Sunrise { get; set; }

        [JsonPropertyName("sunset")]
        public string Sunset { get; set; }

        [JsonPropertyName("solar_noon")]
        public string SolarNoon { get; set; }

        [JsonPropertyName("day_length")]
        public string DayLength { get; set; }

        [JsonPropertyName("civil_twilight_begin")]
        public string CivilTwilightBegin { get; set; }

        [JsonPropertyName("civil_twilight_end")]
        public string CivilTwilightEnd { get; set; }

        [JsonPropertyName("nautical_twilight_begin")]
        public string NauticalTwilightBegin { get; set; }

        [JsonPropertyName("nautical_twilight_end")]
        public string NauticalTwilightEnd { get; set; }

        [JsonPropertyName("astronomical_twilight_begin")]
        public string AstronomicalTwilightBegin { get; set; }

        [JsonPropertyName("astronomical_twilight_end")]
        public string AstronomicalTwilightEnd { get; set; }
    }
}