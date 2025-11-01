[![SonarQube Cloud](https://sonarcloud.io/images/project_badges/sonarcloud-light.svg)](https://sonarcloud.io/summary/new_code?id=rafsanulhasan_n8n_practice)

# n8n_practice
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
