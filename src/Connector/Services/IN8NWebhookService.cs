namespace N8N.Connector.Services;

public interface IN8NWebhookService
{
    Task<WebhookResponse<T>> TriggerWebhookAsync<T>(string webhookUrl, object payload);
}
