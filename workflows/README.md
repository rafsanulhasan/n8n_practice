# n8n Workflows

This directory contains example n8n workflows that demonstrate webhook triggers for various use cases.

## Available Workflows

### 1. Simple Webhook Example (`webhook-example-workflow.json`)
A basic workflow that receives data via webhook and returns a processed response.

**Webhook Path:** `/webhook/simple-webhook`

**Expected Input:**
```json
{
  "message": "Hello from Blazor!",
  "data": {
    "timestamp": "2024-11-01T12:00:00Z",
    "source": "Blazor Frontend"
  }
}
```

**Response:**
```json
{
  "success": true,
  "message": "Data received successfully",
  "receivedData": { ... },
  "processedAt": "2024-11-01T12:00:01Z"
}
```

### 2. User Registration Workflow (`user-registration-workflow.json`)
A workflow that validates user registration data and returns appropriate responses.

**Webhook Path:** `/webhook/user-registration`

**Expected Input:**
```json
{
  "email": "user@example.com",
  "username": "johndoe",
  "password": "securePassword123"
}
```

**Success Response:**
```json
{
  "success": true,
  "message": "User registered successfully",
  "user": {
    "id": "1234567890",
    "email": "user@example.com",
    "username": "johndoe",
    "createdAt": "2024-11-01T12:00:00Z",
    "status": "active"
  }
}
```

**Error Response:**
```json
{
  "success": false,
  "message": "Validation failed",
  "errors": [
    "Invalid email address",
    "Username must be at least 3 characters"
  ]
}
```

### 3. Data Processing Workflow (`data-processing-workflow.json`)
A workflow that processes an array of data items and returns statistics.

**Webhook Path:** `/webhook/process-data`

**Expected Input:**
```json
{
  "items": [
    { "name": "Item1", "value": "100" },
    { "name": "Item2", "value": "200" }
  ]
}
```

**Response:**
```json
{
  "items": [
    {
      "name": "Item1",
      "value": "100",
      "processedAt": "2024-11-01T12:00:00Z",
      "status": "processed"
    },
    {
      "name": "Item2",
      "value": "200",
      "processedAt": "2024-11-01T12:00:00Z",
      "status": "processed"
    }
  ],
  "totalCount": 2,
  "processingTimestamp": "2024-11-01T12:00:00Z",
  "statistics": {
    "totalItems": 2,
    "processedAt": "2024-11-01T12:00:00Z",
    "summary": "Successfully processed 2 items"
  }
}
```

## How to Import Workflows into n8n

1. Start your n8n instance (see main README for setup instructions)
2. Open n8n in your browser (typically `http://localhost:5678`)
3. Click on the "Workflows" menu
4. Click "Import from File" or "Import from URL"
5. Select one of the JSON workflow files from this directory
6. The workflow will be imported and ready to use
7. **Important:** Activate the workflow by clicking the "Inactive" toggle in the top-right corner
8. Your webhook URL will be: `http://localhost:5678/webhook/{webhook-path}`

## Testing Workflows

### Using the Blazor Frontend
The easiest way to test these workflows is using the provided Blazor frontend application (see `n8n-blazor-frontend/` directory).

### Using cURL
You can also test the webhooks using cURL:

```bash
# Simple Webhook Example
curl -X POST http://localhost:5678/webhook/simple-webhook \
  -H "Content-Type: application/json" \
  -d '{"message": "Hello from cURL", "data": {"source": "terminal"}}'

# User Registration
curl -X POST http://localhost:5678/webhook/user-registration \
  -H "Content-Type: application/json" \
  -d '{"email": "test@example.com", "username": "testuser", "password": "password123"}'

# Data Processing
curl -X POST http://localhost:5678/webhook/process-data \
  -H "Content-Type: application/json" \
  -d '{"items": [{"name": "Item1", "value": "100"}, {"name": "Item2", "value": "200"}]}'
```

### Using Postman
1. Import the workflow into n8n and activate it
2. Create a new POST request in Postman
3. Set the URL to `http://localhost:5678/webhook/{webhook-path}`
4. Set Content-Type header to `application/json`
5. Add the JSON payload in the request body
6. Send the request

## Customizing Workflows

You can customize these workflows by:
1. Importing the workflow into n8n
2. Opening the workflow editor
3. Modifying the JavaScript code in the "Code" nodes
4. Adding additional nodes (e.g., database connections, email notifications)
5. Changing validation rules or processing logic
6. Saving your changes

## Workflow Components

Each workflow consists of:
- **Webhook Node**: Receives the HTTP POST request
- **Code Nodes**: Process the data using JavaScript
- **Conditional Nodes** (optional): Route data based on conditions
- **Respond to Webhook Node**: Sends the response back to the caller

## Best Practices

1. **Always validate input data** before processing
2. **Use appropriate error handling** in your code nodes
3. **Keep workflows simple and focused** on a single task
4. **Test thoroughly** before deploying to production
5. **Monitor workflow executions** in the n8n execution log
6. **Secure your webhooks** in production environments (use authentication, HTTPS)
