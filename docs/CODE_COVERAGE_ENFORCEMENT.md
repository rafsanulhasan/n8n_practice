# Code Coverage Enforcement

This document explains how code coverage is enforced in the GitHub Actions CI/CD pipeline.

## Overview

The project enforces a **80% minimum code coverage** threshold for all code changes. This is in addition to the **80% mutation testing** threshold already in place.

## How It Works

### 1. Coverage Collection
- Tests run with XPlat Code Coverage collector
- Coverage reports generated in OpenCover format
- Reports saved to `./coverage` directory

### 2. Coverage Validation
The workflow includes a dedicated step that:
- ✅ Parses coverage reports (OpenCover/Cobertura formats)
- ✅ Calculates overall line coverage percentage
- ✅ Compares against 80% threshold
- ✅ **Fails the build** if coverage is below threshold

### 3. PR Comments
For pull requests, the workflow automatically:
- 📊 Posts a **combined quality gate report** with both coverage and mutation testing
- 🎯 Shows line coverage, branch coverage, and mutation score
- ✅/❌ Indicates pass/fail status for each metric
- 📈 Provides actionable recommendations
- 🔄 Updates the comment on subsequent pushes (no spam)

## Coverage Thresholds

| Type | Threshold | Enforcement |
|------|-----------|-------------|
| **Code Coverage** | **80%** | ✅ GitHub Actions |
| **Mutation Score** | **80%** | ✅ GitHub Actions |
| SonarCloud Coverage | 50% | ✅ SonarCloud Quality Gate |

## Scripts

### check-code-coverage.ps1
Located in `.github/scripts/check-code-coverage.ps1`

**Purpose**: Validates code coverage against threshold

**Usage**:
```powershell
# Check with default 80% threshold
.\check-code-coverage.ps1

# Check with custom threshold
.\check-code-coverage.ps1 -Threshold 75

# Check specific coverage directory
.\check-code-coverage.ps1 -Threshold 80 -CoverageReportsPath "./my-coverage"
```

**Output**:
- ✅ Exit code 0 if coverage >= threshold
- ❌ Exit code 1 if coverage < threshold

### comment-quality-gate.js
Located in `.github/scripts/comment-quality-gate.js`

**Purpose**: Posts combined quality gate report (coverage + mutation testing) as PR comment

**Features**:
- Combines code coverage and mutation testing results in one comment
- Creates new comment or updates existing one
- Shows detailed metrics for both coverage types
- Visual indicators (emojis) for pass/fail status
- Provides actionable recommendations based on results

## GitHub Actions Workflow

The coverage and mutation testing checks are integrated into `.github/workflows/build.yml`:

```yaml
- name: Build and analyze
  run: |
    dotnet build
    dotnet test --no-build --collect:"XPlat Code Coverage" \
      --results-directory ./coverage \
      -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover

- name: Check Code Coverage Threshold
  run: ./.github/scripts/check-code-coverage.ps1 -Threshold 80

- name: Run Mutation Tests
  run: |
    cd tests/Connector.UnitTests
    PROJECT_PATH=$(realpath ../../src/Connector/Connector.csproj)
    dotnet stryker --project "$PROJECT_PATH" --reporter html --reporter json --reporter cleartext

- name: Check Mutation Score
  run: ./.github/scripts/check-mutation-score.ps1 -Threshold 80

- name: Comment Quality Gate Report on PR
  if: github.event_name == 'pull_request'
  uses: actions/github-script@v7
  with:
    script: |
      const commentQualityGate = require('./.github/scripts/comment-quality-gate.js');
      await commentQualityGate({ github, context });
```

**Workflow Order:**
1. Run tests with code coverage collection
2. Validate code coverage threshold (80%)
3. Run mutation tests
4. Validate mutation score threshold (80%)
5. Post combined quality gate report to PR (includes both metrics)

## Local Testing

### Run tests with coverage locally:
```bash
# Run tests and collect coverage
dotnet test --collect:"XPlat Code Coverage" \
  --results-directory ./coverage \
  -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover

# Check coverage threshold
./.github/scripts/check-code-coverage.ps1 -Threshold 80
```

### Generate HTML coverage report:
```bash
# Install ReportGenerator
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate HTML report
reportgenerator \
  -reports:"coverage/**/coverage.opencover.xml" \
  -targetdir:"coverage/report" \
  -reporttypes:Html

# Open report
start coverage/report/index.html  # Windows
open coverage/report/index.html   # macOS
xdg-open coverage/report/index.html  # Linux
```

## Exclusions

The following are excluded from coverage analysis:

- **Blazor WebAssembly projects** (`AgenticChat/**`)
  - Cannot be covered (compile to WASM)
- **Integration test projects** (`AgenticChat.IntegrationTests/**`)
- **Entry points** (`Program.cs`)
- **Generated models** (`N8NModels.cs`)
- **Razor components** (`**/*.razor`)

These exclusions are configured in:
- `sonar-project.properties` (for SonarCloud)
- `.github/workflows/build.yml` (for GitHub Actions)

## Troubleshooting

### Coverage reports not found
**Issue**: Script reports "No coverage reports found"

**Solutions**:
1. Verify tests ran: Check test output in workflow logs
2. Check coverage directory: Ensure `./coverage` contains XML files
3. Verify format: Ensure OpenCover format is specified in test command

### Coverage lower than expected
**Issue**: Coverage percentage seems too low

**Solutions**:
1. Check exclusions: Verify coverage exclusions are correct
2. Review untested code: Use HTML report to identify gaps
3. Add more tests: Focus on uncovered lines/branches

### Build fails on coverage check
**Issue**: Build fails even though you have tests

**Solutions**:
1. Run locally first: Test coverage before pushing
2. Check threshold: Verify 80% is achievable for your project
3. Add more tests: Increase coverage to meet threshold

## Benefits

1. **Quality Assurance**: Ensures new code has adequate test coverage
2. **Early Detection**: Catches low coverage before merge
3. **Visibility**: PR comments provide immediate feedback
4. **Consistency**: Enforces same standards across all contributors
5. **Confidence**: High coverage + mutation testing = robust code

## Related Documentation

- [Mutation Testing Setup](../tests/MUTATION_TESTING_SETUP.md)
- [Test Setup Summary](../tests/TEST_SETUP_SUMMARY.md)
- [Coverage Report](../tests/COVERAGE_REPORT.md)
- [SonarCloud Quality Gate](./configure-sonarqube-quality-gate.md)
