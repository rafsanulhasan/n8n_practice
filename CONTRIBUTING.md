# Contributing to n8n Practice

Thank you for your interest in contributing to this project! This guide will help you get started.

## Ways to Contribute

1. **Add new n8n workflows**
2. **Improve the Blazor frontend**
3. **Fix bugs**
4. **Improve documentation**
5. **Report issues**

## Adding New Workflows

### Step 1: Design in n8n

1. Open n8n at `http://localhost:5678`
2. Create a new workflow
3. Add a Webhook node with a unique path
4. Add processing nodes (Code, IF, etc.)
5. Add a Respond to Webhook node
6. Test the workflow thoroughly

### Step 2: Export the Workflow

1. Click the three dots menu in n8n
2. Select "Download"
3. Save to `workflows/` directory with a descriptive name
4. Format: `{workflow-name}-workflow.json`

### Step 3: Document the Workflow

Add a section to `workflows/README.md` with:
- Workflow name and purpose
- Webhook path
- Expected input format (JSON example)
- Expected output format (JSON example)
- Use cases

### Step 4: Add Blazor Support (Optional)

1. **Create models** in `n8n-blazor-frontend/N8nWebhookClient/Models/N8nModels.cs`:
```csharp
public class MyWorkflowRequest
{
    public string Field1 { get; set; } = string.Empty;
    public string Field2 { get; set; } = string.Empty;
}

public class MyWorkflowResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
```

2. **Add UI section** to `n8n-blazor-frontend/N8nWebhookClient/Pages/N8nWebhooks.razor`:
```razor
<div class="col-md-4">
    <div class="card mb-3">
        <div class="card-header">
            <h5>My Workflow</h5>
        </div>
        <div class="card-body">
            <!-- Add form fields -->
            <button @onclick="TriggerMyWorkflow">Trigger</button>
        </div>
    </div>
</div>

@code {
    private async Task TriggerMyWorkflow()
    {
        var request = new MyWorkflowRequest { /* ... */ };
        var response = await WebhookService.TriggerWebhookAsync<MyWorkflowResponse>(url, request);
        // Handle response
    }
}
```

### Step 5: Test

1. Import the workflow into n8n
2. Activate it
3. Test with cURL or Blazor frontend
4. Verify all edge cases

## Improving Documentation

- Fix typos or unclear instructions
- Add examples or screenshots
- Improve setup guides
- Add troubleshooting tips

## Code Style

### C# (Blazor)
- Follow Microsoft C# coding conventions
- Use meaningful variable names
- Add XML comments for public methods
- Use async/await for asynchronous operations

### JavaScript (n8n)
- Use ES6+ features
- Add comments for complex logic
- Handle errors gracefully
- Validate all inputs

### Razor Components
- Keep components focused and reusable
- Use proper HTML semantics
- Follow Bootstrap conventions
- Add loading states for async operations

## Pull Request Process

1. **Fork** the repository
2. **Create a branch** with a descriptive name:
   - `feature/add-email-workflow`
   - `fix/registration-validation`
   - `docs/improve-setup-guide`
3. **Make your changes** following the guidelines above
4. **Test thoroughly**
5. **Commit** with clear messages:
   - `Add email notification workflow`
   - `Fix validation bug in user registration`
   - `Update setup documentation with screenshots`
6. **Push** to your fork
7. **Open a Pull Request** with:
   - Clear description of changes
   - Why the change is needed
   - How to test the changes

## Testing Guidelines

### For Workflows
- Test with valid inputs
- Test with invalid inputs (edge cases)
- Test error handling
- Verify response format matches documentation

### For Blazor Components
- Build the project: `dotnet build`
- Run the application: `dotnet run`
- Test in browser
- Check browser console for errors
- Test responsive design (mobile, tablet, desktop)

## Reporting Issues

When reporting issues, please include:

1. **Description**: Clear description of the problem
2. **Steps to Reproduce**: Step-by-step instructions
3. **Expected Behavior**: What should happen
4. **Actual Behavior**: What actually happens
5. **Environment**:
   - OS (Windows, macOS, Linux)
   - .NET version: `dotnet --version`
   - n8n version
   - Browser (if frontend issue)
6. **Screenshots**: If applicable
7. **Logs**: Error messages or stack traces

## Project Structure

```
n8n_practice/
├── workflows/              # n8n workflow JSON files
├── n8n-blazor-frontend/   # Blazor WebAssembly app
│   └── N8nWebhookClient/
│       ├── Models/        # Request/response models
│       ├── Services/      # HTTP client services
│       └── Pages/         # Blazor pages/components
├── docs/                  # Documentation
│   ├── SETUP.md          # Setup guide
│   └── ARCHITECTURE.md   # Architecture docs
└── README.md             # Main documentation
```

## Questions?

- Check existing documentation in `docs/`
- Review existing workflows and code
- Open an issue for discussion
- Read n8n documentation: https://docs.n8n.io/

## Code of Conduct

- Be respectful and inclusive
- Provide constructive feedback
- Help others learn and grow
- Focus on the project goals

## License

By contributing, you agree that your contributions will be licensed under the same license as the project (see LICENSE file).

Thank you for contributing! 🎉
