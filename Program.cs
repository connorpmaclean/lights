using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using Lights.Lifx;
using Lights.Sunset;

namespace Lights
{
    public class Program
    {

        private const int LIGHT_KELVIN_HIGH = 5500;
        private const int LIGHT_KELVIN_LOW = 3000;

        private static HttpClient httpClient;

        static async Task Main(string[] args)
        {
            httpClient = new HttpClient();
            SunsetClient sunsetClient = new SunsetClient(httpClient);
            LifxClient lifxClient = new LifxClient(httpClient);

            var allLights = await lifxClient.GetAllLights();

            int desiredKelvin = await GetDesiredKelvin(sunsetClient);
            Console.WriteLine(desiredKelvin);

            var changes = new List<State>();

            foreach (Light light in allLights)
            {
                if (light.IsOn())
                {
                    if (light.Color.Kelvin == desiredKelvin)
                    {
                        Console.WriteLine($"Skipping {light.Label} because Kelvinj is already {desiredKelvin}.");
                    }
                    else
                    {
                        Console.WriteLine($"Changing {light.Label} from {light.Color.Kelvin} to {desiredKelvin}");
                        changes.Add(new State()
                        {
                            Selector = $"label:{light.Label}",
                            Color = $"kelvin:{desiredKelvin}"
                        });
                    }
                }
                else
                {
                    Console.WriteLine($"Skipping {light.Label} because they are off.");
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


        private static async Task<int> GetDesiredKelvin(SunsetClient sunsetClient)
        {
            DateTimeOffset endTime = await sunsetClient.GetSeattleSunsetTime();
            DateTimeOffset beginTime =  endTime.AddHours(-2);
            DateTimeOffset now = DateTimeOffset.UtcNow;
            
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
