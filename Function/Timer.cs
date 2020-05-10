
namespace Lights.Function
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Host;
    using Microsoft.Extensions.Logging;

    using Lights.Core;

    public static class TimerTrigger
    {
        // Set WEBSITE_TIME_ZONE to Pacific Standard Time
        // func azure functionapp publish cmlights --csharp
        [FunctionName("LightsPrime")]
        public static async Task LightsPrime([TimerTrigger("*/5 * 16-23 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await LightsCore.Run(log);
        }

        [FunctionName("LightsOfftime")]
        public static async Task LightsOfftime([TimerTrigger("*/30 * 0-15 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await LightsCore.Run(log);
        }
    }
}
