namespace Lights.Function
{
    using Azure.Storage.Blobs;
    using Lights.Core;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;

    public static class TimerTrigger
    {
        // Set WEBSITE_TIME_ZONE to Pacific Standard Time
        // func azure functionapp publish cmlights --csharp
        [Function("LightsTimer")]
        public static async Task Lights([TimerTrigger("*/30 * * * * *", RunOnStartup = true)] TimerInfo timer, FunctionContext context)
        {
            var logger = context.GetLogger("LightsTimer");

            logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            if (await new EnableCheckBlobHandler().IsOn())
            {
                logger.LogInformation($"Lights enabled, will run");
                await LightsCore.Run(logger);
            }
            else
            {
                logger.LogInformation($"Lights not enabled");
            }
        }
    }
}
