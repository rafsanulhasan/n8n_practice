# n8n Blazor Webhook Client

A Blazor WebAssembly application that demonstrates how to trigger n8n workflows using webhooks.

## Features

- **Simple Webhook Trigger**: Send basic data to n8n workflows
- **User Registration**: Submit user registration data with validation
- **Data Processing**: Send arrays of data for processing
- **Real-time Response Display**: View workflow responses immediately
- **Configurable Webhook URLs**: Easily change n8n endpoint URLs

## Prerequisites

- .NET 8.0 or higher SDK
- Running n8n instance (see main README for setup)
- Modern web browser (Chrome, Firefox, Edge, Safari)

## Getting Started

### 1. Build the Application

```bash
cd n8n-blazor-frontend/N8nWebhookClient
dotnet build
```

### 2. Run the Application

```bash
dotnet run
```

The application will start and be available at `http://localhost:5000` (or another port shown in the console).

### 3. Configure n8n Webhooks

Before using the application, ensure your n8n workflows are:
1. Imported into n8n (from the `workflows/` directory)
2. Activated in n8n
3. Running on the expected port (default: 5678)

### 4. Use the Application

1. Open your browser and navigate to the application URL
2. Click on "n8n Webhooks" in the navigation menu
3. Update the webhook URLs if your n8n instance is running on a different host/port
4. Fill in the form fields for the workflow you want to test
5. Click the trigger button
6. View the response from n8n

## Project Structure

```
N8nWebhookClient/
├── Models/
│   └── N8nModels.cs           # Data models for requests/responses
├── Services/
│   └── N8nWebhookService.cs   # Service for making webhook calls
├── Pages/
│   ├── Home.razor             # Home page
│   ├── N8nWebhooks.razor      # Main webhook testing page
│   └── ...
├── Layout/
│   └── NavMenu.razor          # Navigation menu
├── Program.cs                  # Application entry point
└── N8nWebhookClient.csproj    # Project file
```

## Components

### N8nWebhookService

The `N8nWebhookService` class provides a simple interface for making webhook calls:

```csharp
public class N8nWebhookService
{
    public async Task<WebhookResponse<T>> TriggerWebhookAsync<T>(string webhookUrl, object payload)
    {
        // Makes HTTP POST request to n8n webhook
        // Returns typed response with success status
    }
}
```

### N8nWebhooks Page

The main page provides three sections:
1. **Simple Webhook**: For basic webhook testing
2. **User Registration**: For testing user registration workflow
3. **Data Processing**: For testing data processing workflow

Each section includes:
- Configurable webhook URL input
- Form fields for the specific workflow
- Trigger button with loading state
- Response display area

## Configuration

### Default Webhook URLs

The application uses these default URLs (modify as needed):

```
Simple Webhook: http://localhost:5678/webhook/simple-webhook
User Registration: http://localhost:5678/webhook/user-registration
Data Processing: http://localhost:5678/webhook/process-data
```

### CORS Configuration

If you encounter CORS issues when calling n8n webhooks:

1. In n8n, go to Settings → n8n Settings
2. Add your Blazor app URL to the allowed CORS origins
3. Or use n8n's webhook test mode which allows all origins

## Extending the Application

### Adding New Workflows

1. Create a new model in `Models/N8nModels.cs`:
```csharp
public class MyNewRequest
{
    public string Field1 { get; set; }
    public string Field2 { get; set; }
}

public class MyNewResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
}
```

2. Add a new section to `Pages/N8nWebhooks.razor`:
```razor
<div class="col-md-4">
    <div class="card mb-3">
        <div class="card-header">
            <h5>My New Workflow</h5>
        </div>
        <div class="card-body">
            <!-- Add form fields -->
            <button @onclick="TriggerMyNewWorkflow">Trigger</button>
        </div>
    </div>
</div>

@code {
    private async Task TriggerMyNewWorkflow()
    {
        var request = new MyNewRequest { /* ... */ };
        var response = await WebhookService.TriggerWebhookAsync<MyNewResponse>(url, request);
        // Handle response
    }
}
```

### Styling

The application uses Bootstrap 5 for styling. Modify the CSS files in the `wwwroot/css/` directory to customize the appearance.

## Troubleshooting

### Connection Issues

**Problem**: Cannot connect to n8n webhooks

**Solutions**:
1. Verify n8n is running: `http://localhost:5678`
2. Check that workflows are activated in n8n
3. Verify webhook URLs match the paths in your n8n workflows
4. Check browser console for CORS errors

### Response Not Showing

**Problem**: Webhook triggers but no response is displayed

**Solutions**:
1. Check browser console for errors
2. Verify the response format matches the expected model
3. Use browser developer tools to inspect the network request
4. Check n8n workflow execution log for errors

### Build Errors

**Problem**: Application fails to build

**Solutions**:
1. Ensure .NET 8.0+ SDK is installed: `dotnet --version`
2. Clean and rebuild: `dotnet clean && dotnet build`
3. Restore packages: `dotnet restore`

## Production Deployment

Before deploying to production:

1. **Update webhook URLs** to use your production n8n instance
2. **Enable HTTPS** for secure communication
3. **Implement authentication** if needed
4. **Configure CORS** properly on n8n server
5. **Build for release**: `dotnet publish -c Release`
6. **Deploy** the contents of `bin/Release/net8.0/publish/wwwroot/` to your web server

## Further Reading

- [Blazor Documentation](https://docs.microsoft.com/aspnet/core/blazor/)
- [n8n Webhook Documentation](https://docs.n8n.io/integrations/builtin/core-nodes/n8n-nodes-base.webhook/)
- [HttpClient in Blazor](https://docs.microsoft.com/aspnet/core/blazor/call-web-api)
