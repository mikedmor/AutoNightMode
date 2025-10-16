using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AutoNightMode.Services
{
    public class SunsetService
    {
        private const string ApiUrl = "https://api.sunrise-sunset.org/json";

        public async Task<(TimeSpan? Sunset, string? Error)> GetSunsetTimeAsync(double latitude, double longitude)
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetAsync($"{ApiUrl}?lat={latitude}&lng={longitude}&formatted=0");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                
                // Use case-insensitive deserialization
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var result = JsonSerializer.Deserialize<SunsetApiResponse>(json, options);

                if (result?.Results?.Sunset != null)
                {
                    var sunsetUtc = DateTime.Parse(result.Results.Sunset);
                    // Convert UTC to local time
                    var sunsetLocal = sunsetUtc.ToLocalTime();
                    return (sunsetLocal.TimeOfDay, null);
                }

                return (null, "Invalid API response.");
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        private class SunsetApiResponse
        {
            [JsonPropertyName("results")]
            public SunsetResults? Results { get; set; }
            
            [JsonPropertyName("status")]
            public string? Status { get; set; }
        }

        private class SunsetResults
        {
            [JsonPropertyName("sunset")]
            public string? Sunset { get; set; }
        }
    }
}
