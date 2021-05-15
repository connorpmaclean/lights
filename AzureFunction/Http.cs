namespace Lights.Function
{
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Azure.Functions.Worker.Http;
    using Microsoft.Extensions.Logging;
    using System.Net;
    using System.Threading.Tasks;

    public static class Http
    {
        [Function("Enable")]
        public static async Task<HttpResponseData> Enable([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestData req,
            FunctionContext context)
        {
            var logger = context.GetLogger("Enable");

            logger.LogInformation("Enable request");

            await new EnableCheckBlobHandler().TurnOnOrOff(true);

            logger.LogInformation("Turned on lights");
            return req.CreateResponse(HttpStatusCode.OK);
        }

        [Function("Disable")]
        public static async Task<HttpResponseData> Disable([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestData req,
            FunctionContext context)
        {
            var logger = context.GetLogger("Disable");

            logger.LogInformation("Disable request");

            await new EnableCheckBlobHandler().TurnOnOrOff(false);

            logger.LogInformation("Turned off lights");
            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
