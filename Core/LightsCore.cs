namespace Lights.Core
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Console;
    using Newtonsoft.Json;
    using Lights.Core.Lifx;
    using Lights.Core.Sunset;
    using TimeZoneConverter;

    public class LightsCore
    {
        private const int LIGHT_KELVIN_HIGH = 5500;
        private const int LIGHT_KELVIN_LOW = 3000;

        private static HttpClient httpClient;

        public static TimeZoneInfo PacificTimeZone
        {
            get
            {
                string windowsPstTimeZoneId = "Pacific Standard Time";
                string timeZoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? windowsPstTimeZoneId
                    : TZConvert.WindowsToIana(windowsPstTimeZoneId);

                return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
        }

        public static async Task Run(ILogger logger = null)
        {
            if (logger == null)
            {
                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
                });

                logger = loggerFactory.CreateLogger<LightsCore>();
            }

            httpClient = new HttpClient();
            SunsetClient sunsetClient = new SunsetClient(httpClient);
            LifxClient lifxClient = new LifxClient(httpClient);

            var getAllLightsTask = lifxClient.GetAllLights();

            int desiredKelvin = await GetDesiredKelvin(sunsetClient, logger);
            logger.LogInformation($"Desired kelvin: {desiredKelvin}");

            var changes = new List<State>();
            var allLightsResult = await getAllLightsTask;
            foreach (Light light in allLightsResult)
            {
                if (light.Color.Kelvin == desiredKelvin)
                {
                    logger.LogInformation($"Skipping {light.Label} because Kelvin is already {desiredKelvin}.");
                }
                else if (light.Color.Saturation > 0)
                {
                    logger.LogInformation($"Skipping {light.Label} because saturation is greater than zero ({light.Color.Saturation}) indicating a custom colour is set.");
                }
                else
                {
                    logger.LogInformation($"Changing {light.Label} from {light.Color.Kelvin} to {desiredKelvin}");
                    changes.Add(new State()
                    {
                        Selector = $"label:{light.Label}",
                        Color = $"kelvin:{desiredKelvin}"
                    });
                }
            }

            if (changes.Any())
            {
                var states = new States()
                {
                    Fast = true, // See https://api.developer.lifx.com/v1/docs/set-state#fast-mode
                    StateList = changes
                };

                await lifxClient.PutStates(states);
            }
        }

        private static async Task<int> GetDesiredKelvin(SunsetClient sunsetClient, ILogger logger)
        {
            var sunsetSunrise = await sunsetClient.GetSeattleSunsetTime();

            TimeSpan endSunriseTime = sunsetSunrise.Sunrise;
            TimeSpan beginSunriseTime = endSunriseTime.Add(TimeSpan.FromHours(-2));

            TimeSpan endSunsetTime = sunsetSunrise.Sunset;
            TimeSpan beginSunsetTime =  endSunsetTime.Add(TimeSpan.FromHours(-2));
            TimeSpan now = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow, 
                PacificTimeZone)
                .TimeOfDay;

            logger.LogInformation($"Time now: {now}, sunrise: {endSunriseTime}, sunset: {endSunsetTime}.");
            
            if (now < beginSunriseTime)
            {
                return LIGHT_KELVIN_LOW;
            }
            else if (now < endSunriseTime)
            {
                return (int)Map(now.Ticks, beginSunriseTime.Ticks, endSunriseTime.Ticks, LIGHT_KELVIN_LOW, LIGHT_KELVIN_HIGH);
            }
            else if (now < beginSunsetTime)
            {
                return LIGHT_KELVIN_HIGH;
            }
            else if (now < endSunsetTime)
            {
                return (int)Map(now.Ticks, beginSunsetTime.Ticks, endSunsetTime.Ticks, LIGHT_KELVIN_HIGH, LIGHT_KELVIN_LOW);
            }
            else
            {
                return LIGHT_KELVIN_LOW;
            }
        }

        private static double Map(double value, double fromSource, double toSource, double fromTarget, double toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }
    }
}
