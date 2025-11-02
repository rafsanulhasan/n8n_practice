# Architecture Overview

This document provides a detailed overview of the n8n Practice project architecture, components, and data flow.

## System Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                         User Browser                             │
│                                                                   │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │         Blazor WebAssembly Application                    │  │
│  │                                                            │  │
│  │  ┌───────────────┐  ┌──────────────┐  ┌──────────────┐  │  │
│  │  │  UI Pages     │  │   Services   │  │    Models    │  │  │
│  │  │  (Razor)      │→ │  (Webhook)   │  │  (Request/   │  │  │
│  │  │               │  │              │  │   Response)  │  │  │
│  │  └───────────────┘  └──────┬───────┘  └──────────────┘  │  │
│  │                             │                             │  │
│  └─────────────────────────────┼─────────────────────────────┘  │
└─────────────────────────────────┼─────────────────────────────────┘
                                  │
                                  │ HTTP POST (JSON)
                                  │
                                  ▼
            ┌─────────────────────────────────────────┐
            │         n8n Server (localhost:5678)      │
            │                                           │
            │  ┌────────────────────────────────────┐ │
            │  │        Webhook Endpoint            │ │
            │  │  /webhook/{workflow-path}          │ │
            │  └────────────┬───────────────────────┘ │
            │               │                          │
            │               ▼                          │
            │  ┌────────────────────────────────────┐ │
            │  │         n8n Workflow               │ │
            │  │                                    │ │
            │  │  ┌──────────┐  ┌──────────┐      │ │
            │  │  │ Webhook  │→ │   Code   │      │ │
            │  │  │  Node    │  │   Node   │      │ │
            │  │  └──────────┘  └────┬─────┘      │ │
            │  │                      │            │ │
            │  │                      ▼            │ │
            │  │  ┌──────────┐  ┌──────────┐      │ │
            │  │  │   IF     │→ │ Respond  │      │ │
            │  │  │  Node    │  │   Node   │      │ │
            │  │  └──────────┘  └──────────┘      │ │
            │  │                                    │ │
            │  └────────────────────────────────────┘ │
            │                                           │
            └───────────────────┬───────────────────────┘
                                │
                                │ HTTP Response (JSON)
                                │
                                ▼
            ┌─────────────────────────────────────────┐
            │         Blazor Application               │
            │      (Display Response in UI)            │
            └─────────────────────────────────────────┘
```

## Components

### 1. Blazor Frontend Application

**Technology**: Blazor WebAssembly (Client-side .NET)

**Location**: `n8n-blazor-frontend/N8nWebhookClient/`

**Key Components**:

#### Pages (`Pages/`)
- **N8nWebhooks.razor**: Main page with three workflow testing sections
  - Simple Webhook section
  - User Registration section
  - Data Processing section
  - Form inputs and response displays

#### Services (`Services/`)
- **N8nWebhookService.cs**: HTTP client wrapper for webhook calls
  - `TriggerWebhookAsync<T>()`: Generic method for webhook POST requests
  - Error handling and response parsing
  - Logging support

#### Models (`Models/`)
- **N8nModels.cs**: Data transfer objects (DTOs)
  - Request models (SimpleWebhookRequest, UserRegistrationRequest, etc.)
  - Response models (SimpleWebhookResponse, UserRegistrationResponse, etc.)
  - Strongly typed for compile-time safety

#### Configuration (`Program.cs`)
- Dependency injection setup
- Service registration
- HttpClient configuration

### 2. n8n Workflow Engine

**Technology**: Node.js-based workflow automation tool

**Location**: External service (Docker or npm installation)

**Default Port**: 5678

**Workflows**: Stored in `workflows/` directory as JSON files

#### Workflow Structure

Each workflow consists of:

1. **Webhook Node** (Trigger)
   - HTTP Method: POST
   - Path: Unique identifier (e.g., `/webhook/simple-webhook`)
   - Response Mode: responseNode (waits for explicit response)

2. **Code Nodes** (Processing)
   - JavaScript execution environment
   - Access to `$input.item.json` for webhook data
   - Data transformation and validation
   - Business logic implementation

3. **Conditional Nodes** (Optional)
   - IF nodes for branching logic
   - Route data based on conditions
   - Success vs error paths

4. **Respond to Webhook Node** (Output)
   - Sends JSON response back to caller
   - Configurable response body
   - HTTP status code handling

## Data Flow

### Request Flow

1. **User Interaction**
   ```
   User fills form → Clicks button → Event handler triggered
   ```

2. **Blazor Processing**
   ```
   Event handler → Creates request model → Calls N8nWebhookService
   ```

3. **HTTP Request**
   ```
   Service serializes model to JSON → Posts to n8n webhook URL
   ```

4. **n8n Processing**
   ```
   Webhook receives request → Triggers workflow → Processes through nodes
   ```

5. **Response Flow**
   ```
   n8n responds → Service deserializes → UI updates → User sees result
   ```

### Example: User Registration Flow

```
1. User Input:
   {
     "email": "user@example.com",
     "username": "johndoe",
     "password": "password123"
   }

2. Blazor Service:
   - Wraps in UserRegistrationRequest model
   - POSTs to http://localhost:5678/webhook/user-registration

3. n8n Webhook Node:
   - Receives POST request
   - Passes data to next node

4. n8n Validation Node (Code):
   - Validates email format
   - Checks username length (min 3)
   - Checks password length (min 8)
   - Returns { valid: true/false, errors: [...] }

5. n8n IF Node:
   - Routes to Success path if valid === true
   - Routes to Error path if valid === false

6. n8n Response Node:
   - Success: Returns user object with ID
   - Error: Returns error array

7. Blazor Response:
   - Deserializes to UserRegistrationResponse
   - Displays in UI (formatted JSON)
```

## Communication Protocol

### Request Format

**Method**: HTTP POST

**Headers**:
```
Content-Type: application/json
```

**Body**: JSON payload specific to workflow

Example:
```json
{
  "message": "Hello from Blazor",
  "data": {
    "timestamp": "2024-11-01T12:00:00Z",
    "source": "Blazor Frontend"
  }
}
```

### Response Format

**Status Code**: 200 OK (success) or 4xx/5xx (error)

**Headers**:
```
Content-Type: application/json
```

**Body**: JSON response from n8n workflow

Example Success:
```json
{
  "success": true,
  "message": "Data received successfully",
  "receivedData": { ... },
  "processedAt": "2024-11-01T12:00:01Z"
}
```

Example Error:
```json
{
  "success": false,
  "message": "Validation failed",
  "errors": ["Invalid email", "Password too short"]
}
```

## Security Considerations

### Current Implementation (Development)

- No authentication required
- HTTP (not HTTPS)
- localhost access only
- All data visible in network tab

### Production Recommendations

1. **Authentication**
   - Add API keys or OAuth tokens
   - Implement in both n8n webhook and Blazor service
   - Use HTTP headers for token transmission

2. **HTTPS**
   - Deploy n8n behind reverse proxy (nginx/Apache)
   - Use SSL/TLS certificates
   - Update Blazor URLs to https://

3. **Input Validation**
   - Validate all inputs in Blazor (client-side)
   - Validate all inputs in n8n (server-side)
   - Sanitize data to prevent injection attacks

4. **Rate Limiting**
   - Implement rate limiting in n8n or reverse proxy
   - Prevent abuse and DoS attacks

5. **CORS**
   - Configure proper CORS headers
   - Whitelist specific domains only

## Scalability

### Horizontal Scaling

**n8n**:
- Run multiple n8n instances behind load balancer
- Use external database (PostgreSQL) for workflow data
- Share webhook endpoints across instances

**Blazor**:
- Static files, scales easily
- Deploy to CDN for global distribution
- No server-side state to manage

### Vertical Scaling

**n8n**:
- Increase container/server resources
- Optimize workflow code
- Use caching where appropriate

### Performance Optimization

1. **Connection Pooling**
   - Reuse HttpClient instances in Blazor
   - Configure connection limits

2. **Response Caching**
   - Cache responses for idempotent requests
   - Use CDN for static content

3. **Async Processing**
   - Use async/await throughout
   - Non-blocking I/O operations

4. **Workflow Optimization**
   - Minimize node count
   - Optimize JavaScript code
   - Use native nodes when available

## Monitoring and Logging

### n8n Monitoring

- **Execution History**: View in n8n UI
- **Error Logs**: Check workflow execution logs
- **Performance Metrics**: Execution time, success rate

### Blazor Monitoring

- **Console Logging**: Browser developer tools
- **Service Logging**: ILogger interface
- **Network Tab**: Inspect HTTP requests/responses

### Production Monitoring

- Application Performance Monitoring (APM) tools
- Log aggregation (ELK stack, Splunk)
- Alerting on failures or performance issues

## Deployment

### Development Environment

```
localhost:5678 (n8n) ← → localhost:5000 (Blazor)
```

### Production Environment

```
n8n.yourdomain.com (n8n) ← → app.yourdomain.com (Blazor)
                ↑
          (Load Balancer)
                ↑
        (Multiple n8n instances)
```

## Extensibility

### Adding New Workflows

1. **Design** in n8n UI
2. **Export** as JSON
3. **Save** to `workflows/` directory
4. **Create models** in Blazor
5. **Add UI** section to N8nWebhooks.razor
6. **Document** in workflows/README.md

### Adding External Services

n8n can integrate with:
- Databases (PostgreSQL, MySQL, MongoDB)
- APIs (REST, GraphQL)
- Email services (SMTP, SendGrid)
- Cloud storage (AWS S3, Google Drive)
- Messaging (Slack, Discord, Teams)
- CRM systems (Salesforce, HubSpot)

### Custom Business Logic

- Implement in n8n Code nodes (JavaScript)
- Or call external APIs from n8n
- Keep Blazor frontend generic and reusable

## Best Practices

1. **Separation of Concerns**
   - UI logic in Blazor
   - Business logic in n8n
   - Data validation in both

2. **Error Handling**
   - Graceful degradation
   - User-friendly error messages
   - Detailed logging for debugging

3. **Testing**
   - Test workflows independently (cURL, Postman)
   - Test Blazor components in isolation
   - Integration testing with real n8n instance

4. **Documentation**
   - Document workflow inputs/outputs
   - Document API contracts
   - Keep README files updated

5. **Version Control**
   - Version workflow JSON files
   - Tag releases
   - Use branches for features

## Technology Stack Summary

| Component | Technology | Version | Purpose |
|-----------|-----------|---------|---------|
| Frontend Framework | Blazor WebAssembly | .NET 9.0 | UI and client logic |
| Workflow Engine | n8n | Latest | Automation and processing |
| Programming Language (Frontend) | C# | 12.0 | Type-safe frontend code |
| Programming Language (Workflows) | JavaScript | ES2020+ | Workflow logic |
| HTTP Client | HttpClient | .NET | API communication |
| Serialization | System.Text.Json | .NET | JSON handling |
| UI Framework | Bootstrap | 5.3 | Responsive design |
| Container (Optional) | Docker | Latest | n8n deployment |

## Future Enhancements

1. **Authentication System**
   - User login/registration
   - Role-based access control
   - JWT token management

2. **Real-time Updates**
   - SignalR integration
   - Live workflow status updates
   - Push notifications

3. **Batch Processing**
   - Upload CSV files
   - Process multiple items
   - Download results

4. **Workflow Management**
   - Import/export workflows from UI
   - Workflow version control
   - Workflow templates library

5. **Analytics Dashboard**
   - Execution statistics
   - Success/failure rates
   - Performance metrics
