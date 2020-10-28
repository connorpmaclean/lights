For local testing
```
az group create --name cmlighttest --location westus

az deployment group create --resource-group cmlighttest --template-file template.json
```