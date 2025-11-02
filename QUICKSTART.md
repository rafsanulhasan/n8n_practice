# Quick Start Guide

Get up and running with n8n workflows and Blazor webhook integration in 5 minutes!

## Prerequisites

- Docker (for n8n) OR Node.js 18+
- .NET 8.0+ SDK
- Web browser

## 1. Start n8n (Choose One)

### Using Docker (Easiest):
```bash
docker run -it --rm --name n8n -p 5678:5678 n8nio/n8n
```

### Using npm:
```bash
npm install n8n -g
n8n start
```

## 2. Setup n8n

1. Open browser to: `http://localhost:5678`
2. Create account (stored locally)
3. Click "Workflows" → "Import from File"
4. Import all workflows from `workflows/` directory:
   - `webhook-example-workflow.json`
   - `user-registration-workflow.json`
   - `data-processing-workflow.json`
5. **Activate each workflow** (click "Inactive" toggle)

## 3. Run Blazor App

```bash
cd n8n-blazor-frontend/N8nWebhookClient
dotnet run
```

## 4. Test It!

1. Open Blazor app (URL shown in console)
2. Navigate to "n8n Webhooks"
3. Fill in a form and click "Trigger Webhook"
4. See the response from n8n!

## Testing with cURL

```bash
# Simple test
curl -X POST http://localhost:5678/webhook/simple-webhook \
  -H "Content-Type: application/json" \
  -d '{"message": "Hello n8n!"}'
```

## Troubleshooting

**Workflow returns 404?**
- Make sure workflow is activated in n8n (green toggle)

**Can't connect to n8n?**
- Verify n8n is running: `http://localhost:5678`
- Check the port isn't already in use

**Blazor won't build?**
- Verify .NET version: `dotnet --version` (need 8.0+)
- Try: `dotnet clean && dotnet restore && dotnet build`

## Next Steps

- Read [README.md](../README.md) for detailed information
- Read [docs/SETUP.md](../docs/SETUP.md) for complete setup guide
- Read [docs/ARCHITECTURE.md](../docs/ARCHITECTURE.md) to understand the system

## Available Workflows

| Workflow | Path | Purpose |
|----------|------|---------|
| Simple Webhook | `/webhook/simple-webhook` | Basic data processing |
| User Registration | `/webhook/user-registration` | User validation with errors |
| Data Processing | `/webhook/process-data` | Batch data transformation |

Happy automating! 🚀
