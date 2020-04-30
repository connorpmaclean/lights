namespace Lights.Lifx
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Text;

    using Newtonsoft.Json;

    public class LifxClient
    {
        private const string ACCESS_TOKEN = "c5df807c41d0c73aac5e0b9ee830954b4b33226ab42a0f627fa5ac75e12045e3";

        private HttpClient client;

        public LifxClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<IEnumerable<Light>> GetAllLights()
        {
            var json = await CallApi("https://api.lifx.com/v1/lights/all", HttpMethod.Get, null);
            return JsonConvert.DeserializeObject<IEnumerable<Light>>(json);
        }

        public async Task PutStates(States states)
        {
            string json = JsonConvert.SerializeObject(states);
            await CallApi("https://api.lifx.com/v1/lights/states", HttpMethod.Put, json);
        }

        private async Task<string> CallApi(string url, HttpMethod method, string body)
        {
            HttpRequestMessage message = new HttpRequestMessage(method, url);
            message.Headers.Add("Authorization", $"Bearer {ACCESS_TOKEN}");

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