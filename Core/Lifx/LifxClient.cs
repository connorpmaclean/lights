namespace Lights.Core.Lifx
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class LifxClient
    {

        private HttpClient client;

        public LifxClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<IEnumerable<Light>> GetAllLights()
        {
            var json = await CallApi("https://api.lifx.com/v1/lights/all", HttpMethod.Get, null);
            return JsonSerializer.Deserialize<IEnumerable<Light>>(json);
        }

        public async Task PutStates(States states)
        {
            string json = JsonSerializer.Serialize(states);
            await CallApi("https://api.lifx.com/v1/lights/states", HttpMethod.Put, json);
        }

        private async Task<string> CallApi(string url, HttpMethod method, string body)
        {
            string accessToken = Environment.GetEnvironmentVariable("LifxApiKey");

            HttpRequestMessage message = new HttpRequestMessage(method, url);
            message.Headers.Add("Authorization", $"Bearer {accessToken}");

            if (body != null)
            {
                message.Content = new StringContent(body, Encoding.UTF8, "application/json");
            }

            var response = await this.client.SendAsync(message);
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(response.StatusCode.ToString());
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}