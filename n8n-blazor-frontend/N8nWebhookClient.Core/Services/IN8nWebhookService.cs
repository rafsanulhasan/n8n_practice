namespace N8nWebhookClient.Core.Services;

public interface IN8nWebhookService
{
    Task<N8nWebhookClient.Services.WebhookResponse<T>> TriggerWebhookAsync<T>(string webhookUrl, object payload);
}
