# Setup Guide

This guide will walk you through setting up the n8n Practice repository step by step.

## Table of Contents
1. [Install Prerequisites](#install-prerequisites)
2. [Set Up n8n](#set-up-n8n)
3. [Import Workflows](#import-workflows)
4. [Set Up Blazor Frontend](#set-up-blazor-frontend)
5. [Test the Integration](#test-the-integration)
6. [Next Steps](#next-steps)

## Install Prerequisites

### For n8n

Choose one option:

**Option 1: Docker (Easiest)**
```bash
# Install Docker from https://www.docker.com/get-started
# Verify installation
docker --version
```

**Option 2: Node.js**
```bash
# Install Node.js 18+ from https://nodejs.org/
# Verify installation
node --version
npm --version
```

### For Blazor Frontend

```bash
# Install .NET 8.0 SDK from https://dotnet.microsoft.com/download
# Verify installation
dotnet --version
```

## Set Up n8n

### Using Docker (Recommended)

1. **Start n8n container**:
```bash
docker run -it --rm \
  --name n8n \
  -p 5678:5678 \
  -v ~/.n8n:/home/node/.n8n \
  n8nio/n8n
```

2. **Access n8n**:
   - Open browser to `http://localhost:5678`
   - Create an account (stored locally)
   - Complete the setup wizard

### Using npm

1. **Install n8n globally**:
```bash
npm install n8n -g
```

2. **Start n8n**:
```bash
n8n start
```

3. **Access n8n**:
   - Open browser to `http://localhost:5678`
   - Create an account (stored locally)
   - Complete the setup wizard

### Verify n8n is Running

```bash
# Should return n8n status page
curl http://localhost:5678
```

## Import Workflows

1. **Open n8n** in your browser (`http://localhost:5678`)

2. **Import Simple Webhook Workflow**:
   - Click "Workflows" in the left sidebar
   - Click "Add workflow" dropdown → "Import from File"
   - Select `workflows/webhook-example-workflow.json`
   - Click "Open" or "Import"
   - Click the "Inactive" toggle to activate the workflow

3. **Import User Registration Workflow**:
   - Repeat the above steps for `workflows/user-registration-workflow.json`
   - Activate the workflow

4. **Import Data Processing Workflow**:
   - Repeat the above steps for `workflows/data-processing-workflow.json`
   - Activate the workflow

5. **Verify Workflows**:
   - Go to "Workflows" in the sidebar
   - You should see three workflows, all marked as "Active"
   - Click on each workflow to view it

### Get Webhook URLs

For each workflow:
1. Open the workflow in n8n
2. Click on the "Webhook" node
3. Note the "Production URL" or "Test URL"
4. The URL format is: `http://localhost:5678/webhook/{webhook-path}`

Example URLs:
- Simple Webhook: `http://localhost:5678/webhook/simple-webhook`
- User Registration: `http://localhost:5678/webhook/user-registration`
- Data Processing: `http://localhost:5678/webhook/process-data`

## Set Up Blazor Frontend

1. **Navigate to the frontend directory**:
```bash
cd n8n-blazor-frontend/N8nWebhookClient
```

2. **Restore dependencies**:
```bash
dotnet restore
```

3. **Build the project**:
```bash
dotnet build
```

4. **Run the application**:
```bash
dotnet run
```

5. **Access the application**:
   - Note the URL in the console output (e.g., `http://localhost:5000`)
   - Open this URL in your browser

## Test the Integration

### Test 1: Simple Webhook

1. Open the Blazor app in your browser
2. Navigate to "n8n Webhooks" in the navigation menu
3. In the "Simple Webhook" section:
   - Verify the webhook URL is correct
   - Enter a message in the "Message" field
   - Click "Trigger Webhook"
4. Wait for the response to appear
5. Verify the response shows success and your data

**Expected Response**:
```json
{
  "success": true,
  "message": "Data received successfully",
  "receivedData": {
    "message": "Your message",
    "data": { ... }
  },
  "processedAt": "2024-11-01T..."
}
```

### Test 2: User Registration

1. In the "User Registration" section:
   - Verify the webhook URL is correct
   - Enter an email: `test@example.com`
   - Enter a username: `testuser` (at least 3 characters)
   - Enter a password: `password123` (at least 8 characters)
   - Click "Register User"
2. Wait for the response
3. Verify the response shows success and user data

**Expected Response**:
```json
{
  "success": true,
  "message": "User registered successfully",
  "user": {
    "id": "...",
    "email": "test@example.com",
    "username": "testuser",
    "createdAt": "...",
    "status": "active"
  }
}
```

### Test 3: Invalid Registration (Error Handling)

1. Try registering with invalid data:
   - Email: `invalid-email`
   - Username: `ab` (too short)
   - Password: `short` (too short)
   - Click "Register User"
2. Verify you receive an error response with validation messages

### Test 4: Data Processing

1. In the "Data Processing" section:
   - Verify the webhook URL is correct
   - Click "Process Data" (uses sample data)
2. Wait for the response
3. Verify the response shows processed items and statistics

### Test 5: Using cURL (Alternative)

You can also test directly with cURL:

```bash
# Test simple webhook
curl -X POST http://localhost:5678/webhook/simple-webhook \
  -H "Content-Type: application/json" \
  -d '{"message": "Test from cURL"}'

# Test user registration
curl -X POST http://localhost:5678/webhook/user-registration \
  -H "Content-Type: application/json" \
  -d '{"email": "curl@example.com", "username": "curluser", "password": "password123"}'
```

### Verify in n8n

1. Go back to n8n in your browser
2. Click "Executions" in the left sidebar
3. You should see execution records for each webhook trigger
4. Click on an execution to see detailed information
5. Verify the workflow executed successfully

## Next Steps

### Customize Workflows

1. **Modify existing workflows**:
   - Open a workflow in n8n
   - Click on "Code" nodes to edit JavaScript
   - Change validation rules or processing logic
   - Save and test your changes

2. **Add new nodes**:
   - Drag new nodes from the left panel
   - Connect nodes together
   - Configure node settings
   - Test the modified workflow

### Create New Workflows

1. **Design in n8n**:
   - Click "Add workflow"
   - Add a Webhook node (set the path)
   - Add processing nodes
   - Add a Respond to Webhook node
   - Connect all nodes
   - Activate the workflow

2. **Add to Blazor**:
   - Create models in `Models/N8nModels.cs`
   - Add UI section in `Pages/N8nWebhooks.razor`
   - Implement trigger method
   - Test the integration

3. **Document**:
   - Export the workflow JSON from n8n
   - Save to `workflows/` directory
   - Update `workflows/README.md`

### Deploy to Production

1. **Deploy n8n**:
   - Use n8n Cloud (https://n8n.io/cloud/)
   - Or deploy self-hosted with Docker
   - Configure HTTPS and authentication
   - Update webhook URLs

2. **Deploy Blazor**:
   - Build for production: `dotnet publish -c Release`
   - Deploy to Azure, AWS, or other hosting
   - Update webhook URLs to production n8n instance
   - Configure CORS if needed

### Add Security

1. **Webhook authentication**:
   - Add authentication headers in n8n
   - Update Blazor service to include headers
   - Use API keys or tokens

2. **HTTPS**:
   - Enable HTTPS in n8n (reverse proxy recommended)
   - Use HTTPS for Blazor deployment

3. **Input validation**:
   - Validate all user inputs in Blazor
   - Validate all webhook inputs in n8n
   - Sanitize data to prevent injection attacks

## Troubleshooting

### n8n Won't Start

**Problem**: Port 5678 is already in use

**Solution**:
```bash
# Find process using the port
lsof -i :5678
# Kill the process or use a different port
docker run -p 5679:5678 n8nio/n8n
```

### Webhook Returns 404

**Problem**: Workflow not found

**Solutions**:
1. Verify workflow is activated in n8n
2. Check the webhook path matches exactly
3. Check n8n is running on the correct port
4. Test directly in browser: `http://localhost:5678/webhook/simple-webhook`

### CORS Errors

**Problem**: Browser blocks requests to n8n

**Solutions**:
1. n8n webhooks should work by default in test mode
2. For production, add your domain to n8n CORS settings
3. Or use a reverse proxy to avoid CORS

### Blazor Build Fails

**Problem**: Missing dependencies or SDK version

**Solutions**:
```bash
# Update .NET SDK
# Download from https://dotnet.microsoft.com/download

# Clean and restore
dotnet clean
dotnet restore
dotnet build
```

### No Response from Webhook

**Problem**: Workflow executes but no response

**Solutions**:
1. Check n8n execution log for errors
2. Verify "Respond to Webhook" node is connected
3. Check workflow didn't error before reaching response node
4. Look at browser console for errors

## Getting Help

- Check the documentation in `workflows/README.md` and `n8n-blazor-frontend/README.md`
- Review n8n documentation: https://docs.n8n.io/
- Check n8n community forum: https://community.n8n.io/
- Review Blazor documentation: https://docs.microsoft.com/aspnet/core/blazor/

## Success Checklist

- [ ] n8n is running on port 5678
- [ ] All three workflows are imported and activated
- [ ] Blazor frontend builds and runs successfully
- [ ] Simple webhook test returns success
- [ ] User registration test with valid data returns success
- [ ] User registration with invalid data returns errors
- [ ] Data processing test returns processed data
- [ ] Can view executions in n8n dashboard
- [ ] Understand how to modify workflows
- [ ] Know how to add new workflows

Congratulations! You've successfully set up the n8n Practice repository.
