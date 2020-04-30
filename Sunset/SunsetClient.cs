namespace Lights.Sunset
{
    using System.Net.Http;
    using System;
    using System.Globalization;
    using System.Threading.Tasks;

    using Newtonsoft.Json;


    public class SunsetClient
    {
        public const double SEATTLE_LAT = 47.60357D;
        public const double SEATTLE_LON = -122.32945D;
        private HttpClient client;

        private string currentDateKey;

        private DateTimeOffset currentSunset;

        public SunsetClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<DateTimeOffset> GetSeattleSunsetTime()
        {
            DateTime nowPacific = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow, 
                TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));

            string dateKey = nowPacific.ToString("yyyyMMdd");

            if (this.currentDateKey == null || this.currentDateKey != dateKey)
            {
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, $"https://api.sunrise-sunset.org/json?lat={SEATTLE_LAT}&lng={SEATTLE_LON}");
                var response = await client.SendAsync(message);

                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException(response.StatusCode.ToString());
                }

                string json = await response.Content.ReadAsStringAsync();
                SunriseSunsetResult sunriseSunset = JsonConvert.DeserializeObject<SunriseSunsetResult>(json);

                string sunsetString = sunriseSunset.Results.Sunset;

                this.currentSunset = DateTimeOffset.Parse(sunsetString, null, DateTimeStyles.AssumeUniversal);
                this.currentDateKey = dateKey;
            }

            return currentSunset;
        }

    }
}