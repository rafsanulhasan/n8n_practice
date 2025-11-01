[![SonarQube Cloud](https://sonarcloud.io/images/project_badges/sonarcloud-light.svg)](https://sonarcloud.io/summary/new_code?id=rafsanulhasan_n8n_practice)

# n8n_practice

Practicing n8n workflow automation with Blazor webhook integration.

## Overview

This repository demonstrates how to create n8n workflows with webhook triggers and how to call them from a Blazor frontend application. It includes:

- **Example n8n workflows** (Simple webhook, User registration, Data processing)
- **Blazor WebAssembly frontend** for triggering workflows
- **Complete documentation** for setup and usage

## Project Structure

```
n8n_practice/
├── workflows/                    # n8n workflow JSON files
│   ├── webhook-example-workflow.json
│   ├── user-registration-workflow.json
│   ├── data-processing-workflow.json
│   └── README.md                # Workflow documentation
├── n8n-blazor-frontend/         # Blazor WebAssembly application
│   ├── N8nWebhookClient/        # Main Blazor project
│   │   ├── Models/              # Data models
│   │   ├── Services/            # Webhook service
│   │   ├── Pages/               # Blazor pages
│   │   └── ...
│   └── README.md                # Blazor app documentation
├── docs/                         # Additional documentation
└── README.md                    # This file
```

## Prerequisites

### For n8n
- Docker (recommended) OR Node.js 18+ for local installation
- Port 5678 available (default n8n port)

### For Blazor Frontend
- .NET 8.0+ SDK
- Modern web browser

## Quick Start

### Step 1: Set Up n8n

#### Option A: Using Docker (Recommended)

```bash
docker run -it --rm \
  --name n8n \
  -p 5678:5678 \
  -v ~/.n8n:/home/node/.n8n \
  n8nio/n8n
```

#### Option B: Using npm

```bash
npm install n8n -g
n8n start
```

### Step 2: Import Workflows

1. Open n8n in your browser: `http://localhost:5678`
2. Create an account or log in
3. Click "Workflows" → "Import from File"
4. Import the workflow files from the `workflows/` directory:
   - `webhook-example-workflow.json`
   - `user-registration-workflow.json`
   - `data-processing-workflow.json`
5. **Activate each workflow** by clicking the "Inactive" toggle

### Step 3: Run the Blazor Frontend

```bash
cd n8n-blazor-frontend/N8nWebhookClient
dotnet restore
dotnet run
```

Open your browser to the URL shown in the console (typically `http://localhost:5000`).

### Step 4: Test the Webhooks

1. Navigate to the "n8n Webhooks" page in the Blazor app
2. Verify the webhook URLs match your n8n instance
3. Fill in the form fields and click the trigger buttons
4. View the responses from n8n

## Available Workflows

### 1. Simple Webhook Example
**Purpose**: Basic webhook testing  
**Path**: `/webhook/simple-webhook`  
**Use Case**: Send simple messages and data to n8n

### 2. User Registration
**Purpose**: Validate and process user registration  
**Path**: `/webhook/user-registration`  
**Use Case**: User sign-up forms, account creation

### 3. Data Processing
**Purpose**: Transform and analyze data arrays  
**Path**: `/webhook/process-data`  
**Use Case**: Batch data processing, ETL operations

## Detailed Documentation

- **Workflows**: See [workflows/README.md](workflows/README.md)
- **Blazor Frontend**: See [n8n-blazor-frontend/README.md](n8n-blazor-frontend/README.md)

## Testing with cURL

You can test the workflows without the Blazor frontend:

```bash
# Simple Webhook
curl -X POST http://localhost:5678/webhook/simple-webhook \
  -H "Content-Type: application/json" \
  -d '{"message": "Hello", "data": {"source": "curl"}}'

# User Registration
curl -X POST http://localhost:5678/webhook/user-registration \
  -H "Content-Type: application/json" \
  -d '{"email": "test@example.com", "username": "testuser", "password": "password123"}'

# Data Processing
curl -X POST http://localhost:5678/webhook/process-data \
  -H "Content-Type: application/json" \
  -d '{"items": [{"name": "Item1", "value": "100"}]}'
```

## Architecture

### n8n Workflows
- Receive HTTP POST requests via webhook nodes
- Process data using JavaScript code nodes
- Validate input and handle errors
- Return JSON responses

### Blazor Frontend
- WebAssembly application (runs in browser)
- `N8nWebhookService` for HTTP communication
- Typed models for request/response data
- Interactive UI with real-time feedback

### Communication Flow
```
Blazor App → HTTP POST → n8n Webhook → Process → Response → Blazor App
```

## Customization

### Creating New Workflows

1. **Design the workflow** in n8n:
   - Add a Webhook node (trigger)
   - Add processing nodes (Code, IF, etc.)
   - Add a Respond to Webhook node
   - Activate the workflow

2. **Create models** in Blazor (`Models/N8nModels.cs`):
   ```csharp
   public class MyRequest { /* properties */ }
   public class MyResponse { /* properties */ }
   ```

3. **Add UI** in Blazor (`Pages/N8nWebhooks.razor`):
   - Form fields for input
   - Trigger button
   - Response display

4. **Export workflow** from n8n and save to `workflows/` directory

### Extending Functionality

- **Add authentication**: Implement API keys or OAuth in both n8n and Blazor
- **Add database storage**: Use n8n database nodes to persist data
- **Add notifications**: Use n8n email/Slack nodes to send alerts
- **Add file uploads**: Use n8n's HTTP Request node to handle files
- **Add scheduling**: Use n8n's Cron node for scheduled workflows

## Troubleshooting

### n8n Not Starting
- Check if port 5678 is available: `lsof -i :5678`
- Check Docker logs: `docker logs n8n`
- Try a different port: `docker run -p 5679:5678 n8nio/n8n`

### Workflow Not Responding
- Verify workflow is activated (toggle in top-right)
- Check n8n execution log for errors
- Test webhook directly with cURL
- Check n8n server logs

### Blazor CORS Issues
- n8n webhooks should allow CORS by default in test mode
- For production, configure CORS in n8n settings
- Add your Blazor app URL to allowed origins

### Build Errors
- Ensure .NET 8.0+ is installed: `dotnet --version`
- Clean and rebuild: `dotnet clean && dotnet build`
- Check for missing dependencies: `dotnet restore`

## Best Practices

1. **Security**: Never expose sensitive data in webhooks without authentication
2. **Validation**: Always validate input data in n8n workflows
3. **Error Handling**: Implement proper error handling in both n8n and Blazor
4. **Testing**: Test workflows thoroughly before production use
5. **Monitoring**: Monitor n8n execution logs for issues
6. **Documentation**: Document custom workflows and their expected inputs

## Resources

- [n8n Documentation](https://docs.n8n.io/)
- [n8n Webhook Documentation](https://docs.n8n.io/integrations/builtin/core-nodes/n8n-nodes-base.webhook/)
- [Blazor Documentation](https://docs.microsoft.com/aspnet/core/blazor/)
- [n8n Community](https://community.n8n.io/)

## License

See [LICENSE](LICENSE) file for details.

## Contributing

Feel free to submit issues or pull requests to improve the examples and documentation.

Practicing n8n workflow automation

# run sonar locally
## JS/TS and Web
```bash
sonar \
  -Dsonar.host.url=https://lacey-predial-brianne.ngrok-free.dev \
  -Dsonar.token=sqp_606322cdad48a75c60c9d397c54091bb709fe9d2 \
  -Dsonar.projectKey=rafsanulhasan_n8n_practice_02055fdc-226c-434c-a55b-968753cb4f1e
```

## .NET 
```bash
dotnet sonarscanner begin /k:"rafsanulhasan_n8n_practice_02055fdc-226c-434c-a55b-968753cb4f1e" /d:sonar.host.url="https://lacey-predial-brianne.ngrok-free.dev"  /d:sonar.token="sqp_606322cdad48a75c60c9d397c54091bb709fe9d2"
dotnet build
dotnet sonarscanner end /d:sonar.token="sqp_606322cdad48a75c60c9d397c54091bb709fe9d2"
```

## python
```bash
pip install pysonar
pysonar \
  --sonar-host-url=https://lacey-predial-brianne.ngrok-free.dev \
  --sonar-token=sqp_606322cdad48a75c60c9d397c54091bb709fe9d2 \
  --sonar-project-key=rafsanulhasan_n8n_practice_02055fdc-226c-434c-a55b-968753cb4f1e
```
