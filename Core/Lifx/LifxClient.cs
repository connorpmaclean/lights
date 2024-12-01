namespace Lights.Core.Lifx
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    public class LifxClient
    {

        private HttpClient _client;

        private static JsonSerializerOptions s_jsonOptions = new();

        static LifxClient()
        {
            s_jsonOptions.Converters.Add(new NullableDateTimeOffsetParser());
        }

        public LifxClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<Light>> GetAllLights()
        {
            var json = await CallApi("https://api.lifx.com/v1/lights/all", HttpMethod.Get, null);
            return JsonSerializer.Deserialize<IEnumerable<Light>>(json, s_jsonOptions);
        }

        public async Task PutStates(States states)
        {
            string json = JsonSerializer.Serialize(states, s_jsonOptions);
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

            var response = await _client.SendAsync(message);
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(response.StatusCode.ToString());
            }

            return await response.Content.ReadAsStringAsync();
        }

        public class NullableDateTimeOffsetParser : JsonConverter<DateTimeOffset?>
        {
            public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                string value = reader.GetString();
                if (string.IsNullOrEmpty(value))
                {
                    return null;
                }

                return DateTimeOffset.Parse(reader.GetString());
            }

            public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value?.ToString());
            }
        }
    }
}