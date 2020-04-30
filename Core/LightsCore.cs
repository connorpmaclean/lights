namespace Lights.Core
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Console;
    using Newtonsoft.Json;
    using Lights.Core.Lifx;
    using Lights.Core.Sunset;

    public class LightsCore
    {
        private const int LIGHT_KELVIN_HIGH = 5500;
        private const int LIGHT_KELVIN_LOW = 3000;

        private static HttpClient httpClient;

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

            var allLights = await lifxClient.GetAllLights();

            int desiredKelvin = await GetDesiredKelvin(sunsetClient, logger);
            logger.LogInformation($"Desired kelvin: {desiredKelvin}");

            var changes = new List<State>();

            foreach (Light light in allLights)
            {
                if (light.IsOn())
                {
                    if (light.Color.Kelvin == desiredKelvin)
                    {
                        logger.LogInformation($"Skipping {light.Label} because Kelvin is already {desiredKelvin}.");
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
                else
                {
                    logger.LogInformation($"Skipping {light.Label} because they are off.");
                }
            }

            if (changes.Any())
            {
                var states = new States()
                {
                    StateList = changes
                };

                await lifxClient.PutStates(states);
            }
        }

        private static async Task<int> GetDesiredKelvin(SunsetClient sunsetClient, ILogger logger)
        {
            TimeSpan endTime = await sunsetClient.GetSeattleSunsetTime();
            TimeSpan beginTime =  endTime.Add(TimeSpan.FromHours(-2));
            TimeSpan now = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow, 
                TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"))
                .TimeOfDay;

            logger.LogInformation($"Time now: {now}, begin: {beginTime}, end (sunset): {endTime}.");
            
            if (now < beginTime)
            {
                return LIGHT_KELVIN_HIGH;
            }
            else if (now > endTime)
            {
                return LIGHT_KELVIN_LOW;
            }
            else
            {
                return (int)Map(now.Ticks, beginTime.Ticks, endTime.Ticks, LIGHT_KELVIN_HIGH, LIGHT_KELVIN_LOW);
            }
        }

        private static double Map(double value, double fromSource, double toSource, double fromTarget, double toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }
    }
}
