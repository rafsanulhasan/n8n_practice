# Troubleshooting Guide

Common issues and solutions for the n8n Practice project.

## Table of Contents
- [n8n Issues](#n8n-issues)
- [Blazor Issues](#blazor-issues)
- [Workflow Issues](#workflow-issues)
- [Network Issues](#network-issues)
- [Build Issues](#build-issues)

## n8n Issues

### n8n Won't Start

**Problem:** Docker or npm n8n won't start

**Solutions:**

1. **Port already in use:**
   ```bash
   # Check what's using port 5678
   lsof -i :5678
   # Kill the process or use different port
   docker run -p 5679:5678 n8nio/n8n
   ```

2. **Docker not running:**
   ```bash
   # Start Docker Desktop (GUI)
   # Or check Docker daemon status
   docker ps
   ```

3. **npm permission issues:**
   ```bash
   # Install with sudo (Linux/Mac)
   sudo npm install n8n -g
   ```

### n8n Crashes or Stops

**Problem:** n8n stops unexpectedly

**Solutions:**

1. **Check logs:**
   ```bash
   # For Docker
   docker logs n8n
   
   # For npm
   # Check terminal output
   ```

2. **Resource limits:**
   ```bash
   # Increase Docker memory limit in Docker Desktop settings
   # Or allocate more resources:
   docker run -m 2g n8nio/n8n
   ```

### Can't Access n8n UI

**Problem:** Browser can't reach http://localhost:5678

**Solutions:**

1. **Verify n8n is running:**
   ```bash
   curl http://localhost:5678
   # Should return HTML
   ```

2. **Check firewall:**
   - Allow port 5678 through firewall
   - Check antivirus software

3. **Try different browser:**
   - Chrome, Firefox, Edge, Safari
   - Clear browser cache

## Blazor Issues

### Build Fails

**Problem:** `dotnet build` fails

**Solutions:**

1. **Check .NET version:**
   ```bash
   dotnet --version
   # Should be 8.0 or higher
   ```

2. **Restore packages:**
   ```bash
   cd n8n-blazor-frontend/N8nWebhookClient
   dotnet clean
   dotnet restore
   dotnet build
   ```

3. **Update .NET SDK:**
   - Download from https://dotnet.microsoft.com/download
   - Install latest version

### Application Won't Run

**Problem:** `dotnet run` fails or app crashes

**Solutions:**

1. **Port conflict:**
   ```bash
   # Check if port 5000 is in use
   lsof -i :5000
   ```

2. **Build first:**
   ```bash
   dotnet build
   dotnet run
   ```

3. **Check for errors:**
   - Read console output carefully
   - Look for error messages
   - Check browser console (F12)

### Blank Page

**Problem:** Blazor app loads but shows blank page

**Solutions:**

1. **Check browser console:**
   - Press F12
   - Look for JavaScript errors
   - Check Network tab for failed requests

2. **Clear cache:**
   - Hard refresh: Ctrl+F5 (Windows/Linux) or Cmd+Shift+R (Mac)
   - Clear browser cache completely

3. **Check file permissions:**
   - Ensure wwwroot files are readable

## Workflow Issues

### Workflow Returns 404

**Problem:** Webhook URL returns "Not Found"

**Solutions:**

1. **Activate workflow:**
   - Open workflow in n8n
   - Click "Inactive" toggle (should turn green)
   - Verify it says "Active"

2. **Check webhook path:**
   - In n8n, click Webhook node
   - Verify the path matches your URL
   - Example: `/webhook/simple-webhook`

3. **Test URL format:**
   ```bash
   # Correct format
   http://localhost:5678/webhook/simple-webhook
   
   # Wrong formats
   http://localhost:5678/simple-webhook  # Missing /webhook
   http://localhost:5678/webhook/simple-webhook/  # Extra slash
   ```

### Workflow Returns Error

**Problem:** Workflow executes but returns error

**Solutions:**

1. **Check execution log:**
   - In n8n, click "Executions"
   - Find the failed execution
   - Click to see details
   - Check which node failed

2. **Validate input data:**
   ```bash
   # Ensure JSON is valid
   echo '{"email": "test@example.com"}' | jq .
   ```

3. **Check node configuration:**
   - Open workflow
   - Click failing node
   - Verify JavaScript code is correct
   - Test with simpler code

### Workflow Returns Wrong Data

**Problem:** Workflow works but returns unexpected data

**Solutions:**

1. **Test nodes individually:**
   - Click "Execute Node" in n8n
   - Check output of each node
   - Verify data transformation

2. **Check variable names:**
   - Verify `$input.item.json` access
   - Check property names (case-sensitive)

3. **Add debug logging:**
   ```javascript
   console.log('Input data:', $input.item.json);
   // Your code here
   console.log('Output data:', result);
   ```

## Network Issues

### CORS Errors

**Problem:** Browser blocks requests with CORS error

**Solutions:**

1. **n8n should allow CORS by default in test mode**
   - Verify workflow is in test mode
   - Check n8n settings

2. **For production, configure CORS:**
   ```bash
   # Set environment variable
   export N8N_CORS_ORIGINS="https://yourapp.com"
   ```

3. **Use n8n's production webhook:**
   - In webhook node, use production URL
   - Not test URL

### Connection Refused

**Problem:** Cannot connect to n8n from Blazor

**Solutions:**

1. **Verify n8n is running:**
   ```bash
   curl http://localhost:5678/webhook/simple-webhook
   ```

2. **Check URL in Blazor:**
   - Open `Pages/N8nWebhooks.razor`
   - Verify webhook URLs
   - Ensure they match n8n

3. **Check network:**
   - Disable VPN temporarily
   - Check proxy settings
   - Try from same machine

### Slow Response

**Problem:** Webhook takes too long to respond

**Solutions:**

1. **Optimize workflow:**
   - Reduce number of nodes
   - Simplify JavaScript code
   - Remove unnecessary operations

2. **Check system resources:**
   ```bash
   # Check CPU and memory
   top
   # Or
   htop
   ```

3. **Network latency:**
   - Test on localhost first
   - Check internet connection if using remote n8n

## Build Issues

### Missing Dependencies

**Problem:** Build fails with missing package errors

**Solutions:**

```bash
# Restore NuGet packages
cd n8n-blazor-frontend/N8nWebhookClient
dotnet restore --force

# Clear NuGet cache if needed
dotnet nuget locals all --clear
dotnet restore
```

### Razor Compilation Errors

**Problem:** Errors in .razor files

**Solutions:**

1. **Check syntax:**
   - Ensure `@code {}` is at component level
   - Verify all tags are closed
   - Check C# syntax in code blocks

2. **Rebuild:**
   ```bash
   dotnet clean
   dotnet build
   ```

### Bootstrap Not Loading

**Problem:** UI looks unstyled

**Solutions:**

1. **Verify Bootstrap files exist:**
   ```bash
   ls wwwroot/lib/bootstrap/dist/css/
   ```

2. **Check index.html:**
   - Verify CSS links are correct
   - Check paths to Bootstrap files

3. **Clear browser cache:**
   - Hard refresh page

## Getting Help

If none of these solutions work:

1. **Check the documentation:**
   - [README.md](README.md)
   - [docs/SETUP.md](docs/SETUP.md)
   - [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)

2. **Review examples:**
   - Check workflow JSON files
   - Review Blazor code examples
   - Compare with working configurations

3. **Check versions:**
   ```bash
   # Check .NET version
   dotnet --version
   
   # Check Docker version
   docker --version
   
   # Check n8n version
   n8n --version
   ```

4. **Search for similar issues:**
   - n8n community: https://community.n8n.io/
   - Stack Overflow
   - GitHub issues

5. **Create minimal reproduction:**
   - Isolate the problem
   - Test with simplest possible setup
   - Document steps to reproduce

## Debug Mode

### Enable Verbose Logging

**n8n:**
```bash
# Docker
docker run -e N8N_LOG_LEVEL=debug n8nio/n8n

# npm
export N8N_LOG_LEVEL=debug
n8n start
```

**Blazor:**
- Open browser DevTools (F12)
- Check Console tab
- Check Network tab
- Look for red errors

### Test Components Individually

1. **Test n8n directly:**
   ```bash
   curl -X POST http://localhost:5678/webhook/simple-webhook \
     -H "Content-Type: application/json" \
     -d '{"test": "data"}'
   ```

2. **Test Blazor in isolation:**
   - Comment out webhook calls
   - Test UI interactions only

3. **Test with Postman:**
   - Install Postman
   - Create requests for each workflow
   - Compare with Blazor results

## Common Error Messages

### "The type or namespace name could not be found"

**Cause:** Missing using statement or package reference

**Fix:**
```bash
dotnet restore
# Or add missing using statement to code
```

### "Cannot find node type"

**Cause:** n8n node package not installed

**Fix:**
- Workflow uses correct built-in nodes
- Or install custom node package in n8n

### "JsonSerializationException"

**Cause:** Response doesn't match expected model

**Fix:**
- Check actual response in browser DevTools
- Update model to match response
- Or make properties nullable

## Prevention Tips

1. **Keep everything updated:**
   - Regular `dotnet restore`
   - Update n8n periodically
   - Check for security updates

2. **Test frequently:**
   - Test after each change
   - Use curl for quick tests
   - Check n8n execution log

3. **Use version control:**
   - Commit working versions
   - Create branches for experiments
   - Easy to rollback if needed

4. **Document changes:**
   - Note what worked
   - Record configuration values
   - Keep setup notes

## Still Having Issues?

Create an issue with:
- Detailed problem description
- Steps to reproduce
- Expected vs actual behavior
- Version information
- Error messages
- Screenshots if applicable

---

**Last Updated:** 2024-11-01  
**Maintained by:** Project contributors
