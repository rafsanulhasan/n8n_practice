# Mutation Testing with Stryker.NET

This project uses **Stryker.NET** for mutation testing to ensure the quality and effectiveness of our test suite.

## What is Mutation Testing?

Mutation testing is a method to evaluate the quality of your tests by introducing small changes (mutations) to your code and checking if your tests catch these changes. If a test fails, the mutation is "killed." If all tests still pass, the mutation "survived," indicating a potential gap in your test coverage.

## Setup

Stryker.NET is installed as a local .NET tool and configured in `.config/dotnet-tools.json`.

### Installation (Already Done)
```bash
dotnet tool restore
```

## Running Mutation Tests

### Run All Mutation Tests (Recommended)
```powershell
# Windows PowerShell
.\run-mutation-tests.ps1
```

### Run Mutation Tests for Specific Projects

#### Unit Tests Only
```bash
cd n8n-blazor-frontend/N8nWebhookClient.UnitTests
dotnet stryker
```

#### Integration Tests Only
```bash
cd n8n-blazor-frontend/N8nWebhookClient.IntegrationTests
dotnet stryker
```

## Configuration

Each test project has its own `stryker-config.json` file with the following settings:

### Thresholds
- **High**: 80% (Excellent mutation score)
- **Low**: 60% (Acceptable mutation score)
- **Break**: 50% (Build fails if score is below this)

### Mutation Level
- **Standard**: Applies common mutations without being overly aggressive

### Ignored Mutations
- String literals (often not meaningful for test quality)
- ToString/GetHashCode/Equals methods (usually not business logic)

### Reporters
- **HTML**: Interactive web report
- **JSON**: Machine-readable format for CI/CD
- **Progress**: Real-time progress in console
- **Cleartext**: Summary in console

## Understanding Results

### Mutation Scores
- **100%**: All mutations were killed (perfect!)
- **80-99%**: Excellent test quality
- **60-79%**: Good test quality
- **40-59%**: Acceptable, but could be improved
- **<40%**: Poor test quality, needs improvement

### Mutation Status
- **Killed**: ✅ Test detected the mutation (good!)
- **Survived**: ❌ Mutation wasn't detected (test gap)
- **Timeout**: ⏱️ Test took too long (possible infinite loop)
- **No Coverage**: 🚫 Code not covered by tests
- **Compile Error**: 🔨 Mutation caused compilation error (Stryker issue)

## Viewing Reports

After running Stryker, reports are generated in:
```
n8n-blazor-frontend/N8nWebhookClient.UnitTests/StrykerOutput/[timestamp]/reports/mutation-report.html
n8n-blazor-frontend/N8nWebhookClient.IntegrationTests/StrykerOutput/[timestamp]/reports/mutation-report.html
```

Open the HTML report in your browser for an interactive view:
- Click on files to see which mutations survived
- Review code with color-coded annotations
- Identify test gaps and improve coverage

## Common Mutations

Stryker.NET applies various mutations including:

### Arithmetic Operators
```csharp
// Original
result = a + b;

// Mutated
result = a - b;
```

### Boolean Operators
```csharp
// Original
if (a && b)

// Mutated
if (a || b)
```

### Relational Operators
```csharp
// Original
if (x > 5)

// Mutated
if (x >= 5)
```

### Logical Operators
```csharp
// Original
if (!condition)

// Mutated
if (condition)
```

### Assignment
```csharp
// Original
x += 1;

// Mutated
x -= 1;
```

## CI/CD Integration

Mutation tests run automatically in GitHub Actions on:
- Push to `main` branch
- Pull requests

### View Results in GitHub
1. Go to **Actions** tab in GitHub
2. Select the workflow run
3. Download **mutation-test-reports** artifact
4. Extract and open `mutation-report.html` files

## Best Practices

### 1. Run Mutation Tests Regularly
```bash
# Before committing
dotnet stryker

# After adding new tests
dotnet stryker
```

### 2. Aim for High Mutation Scores
- Target **>80%** for critical business logic
- **60-80%** is acceptable for less critical code
- **<60%** indicates test improvements needed

### 3. Fix Survived Mutations
When mutations survive:
1. Review the survived mutation in the HTML report
2. Understand why the test didn't catch it
3. Add or improve tests to kill the mutation
4. Re-run Stryker to verify

### 4. Don't Ignore Low Scores
A low mutation score means:
- Tests exist but don't effectively verify behavior
- Edge cases aren't covered
- Assertions might be too weak

### 5. Balance with Performance
Mutation testing is slower than regular tests:
- Run locally before committing
- Let CI/CD handle comprehensive runs
- Use `--concurrency` to parallelize (already configured)

## Configuration Options

### Customize `stryker-config.json`

```json
{
  "stryker-config": {
    "mutation-level": "complete",  // Options: basic, standard, complete, advanced
    "concurrency": 4,               // Number of parallel processes
    "thresholds": {
      "high": 90,                   // Increase for stricter quality
      "low": 70,
      "break": 60
    }
  }
}
```

### Command Line Options

```bash
# Only mutate specific files
dotnet stryker --mutate "**/MyService.cs"

# Faster run with fewer concurrent processes
dotnet stryker --concurrency 2

# Skip specific mutations
dotnet stryker --ignore-mutations "StringLiteral,BlockStatement"
```

## Troubleshooting

### Stryker Takes Too Long
- Reduce `concurrency` value
- Use `--mutate` to target specific files
- Set `mutation-level` to `basic` for faster runs

### Many Timeouts
- Increase timeout in config: `"timeout": 10000` (milliseconds)
- Check for infinite loops in your code

### Build Fails on CI/CD
- Check threshold settings
- Review survived mutations
- Consider using `continue-on-error: true` initially

## Resources

- [Stryker.NET Documentation](https://stryker-mutator.io/docs/stryker-net/introduction/)
- [Mutation Testing Best Practices](https://stryker-mutator.io/docs/General/example/)
- [Stryker Configuration Options](https://stryker-mutator.io/docs/stryker-net/configuration/)

## Example Workflow

```bash
# 1. Write new code
# 2. Write tests for the code
dotnet test

# 3. Check code coverage
dotnet test --collect:"XPlat Code Coverage"

# 4. Run mutation tests
dotnet stryker

# 5. Review mutation report
# 6. Improve tests to kill survived mutations
# 7. Re-run mutation tests
dotnet stryker

# 8. Commit when mutation score is acceptable
git commit -m "feat: add feature with >80% mutation coverage"
```

## Monitoring Mutation Score Over Time

Track your mutation scores in pull request reviews:
- Set minimum thresholds in `stryker-config.json`
- Review HTML reports in CI/CD artifacts
- Discuss survived mutations in code reviews
- Gradually increase quality thresholds

---

**Remember**: A high mutation score indicates that your tests effectively catch bugs. It's not just about code coverage, but about the **quality** of your tests! 🎯
