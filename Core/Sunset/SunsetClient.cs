namespace Lights.Core.Sunset
{
    using System.Net.Http;
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    using Newtonsoft.Json;
    using TimeZoneConverter;

    public class SunsetClient
    {
        public const double SEATTLE_LAT = 47.60357D;
        public const double SEATTLE_LON = -122.32945D;

        private HttpClient client;

        private string currentDateKey;

        private SunsetSunrise current;

        public SunsetClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<SunsetSunrise> GetSeattleSunsetTime()
        {
            DateTime utcNow = DateTime.UtcNow;
            DateTime nowPacific = TimeZoneInfo.ConvertTimeFromUtc(
                utcNow, 
                LightsCore.PacificTimeZone);

            string dateKey = nowPacific.ToString("yyyyMMdd");

            if (this.currentDateKey == null || this.currentDateKey != dateKey)
            {
                // Homepage: https://sunrise-sunset.org/api
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, $"https://api.sunrise-sunset.org/json?lat={SEATTLE_LAT}&lng={SEATTLE_LON}");
                var response = await client.SendAsync(message);

                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException(response.StatusCode.ToString());
                }

                string json = await response.Content.ReadAsStringAsync();
                SunriseSunsetResult sunriseSunset = JsonConvert.DeserializeObject<SunriseSunsetResult>(json);

                TimeSpan sunrise = ConvertResponseToPSTTime(sunriseSunset.Results.Sunrise);
                TimeSpan sunset = ConvertResponseToPSTTime(sunriseSunset.Results.Sunset);

                this.current = new SunsetSunrise() 
                {
                    Sunrise = sunrise,
                    Sunset = sunset
                };

                this.currentDateKey = dateKey;
            }

            return this.current;
        }

        private static TimeSpan ConvertResponseToPSTTime(string rawResponse)
        {
            DateTime utcSunset = DateTime.Parse(rawResponse, null, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

            return TimeZoneInfo.ConvertTimeFromUtc(
                utcSunset, 
                TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"))
                .TimeOfDay;
        }

        public class SunsetSunrise
        {
            public TimeSpan Sunrise { get; set; }
            public TimeSpan Sunset { get; set; }
        }
    }
}