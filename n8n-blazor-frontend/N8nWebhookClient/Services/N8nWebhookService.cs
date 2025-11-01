using System.Text;
using System.Text.Json;

namespace N8nWebhookClient.Services
{
    public class N8nWebhookService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<N8nWebhookService> _logger;

        public N8nWebhookService(HttpClient httpClient, ILogger<N8nWebhookService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<WebhookResponse<T>> TriggerWebhookAsync<T>(string webhookUrl, object payload)
        {
            try
            {
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInformation($"Sending webhook request to: {webhookUrl}");
                
                var response = await _httpClient.PostAsync(webhookUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var data = JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true 
                    });

                    return new WebhookResponse<T>
                    {
                        Success = true,
                        Data = data,
                        StatusCode = (int)response.StatusCode,
                        RawResponse = responseContent
                    };
                }
                else
                {
                    _logger.LogError($"Webhook request failed with status code: {response.StatusCode}");
                    return new WebhookResponse<T>
                    {
                        Success = false,
                        ErrorMessage = $"Request failed with status code: {response.StatusCode}",
                        StatusCode = (int)response.StatusCode,
                        RawResponse = responseContent
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error triggering webhook");
                return new WebhookResponse<T>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }

    public class WebhookResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public int StatusCode { get; set; }
        public string? RawResponse { get; set; }
    }
}
