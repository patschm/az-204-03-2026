## Registering the Calculator Frontend
To create the Calculator frontend container, you need to build and upload the project to a container registry. The following steps outline the process:
1. **Build the Docker Image**: Use the Docker CLI to build the image from the Dockerfile located in the `ACME.Web.Calculator` directory.
    ```bash
    docker build -t acme-regop -f ACME.Backend.RegisterOperation\Dockerfile .
    ```
2. **Tag the Image**: Tag the image with your container registry URL.
    ```bash 
    docker tag acme-regop <your-registry-url>/acme-regop:v1
    ```
3. **Push the Image**: Push the tagged image to your container registry.
    ```bash
    docker push <your-registry-url>/acme-regop:v1
    ```
Alternatively, you can use the azure cli to build and push the image directly to Azure Container Registry:
```bash 
az acr build --registry <your-registry-name> --image acme-regop:v1 --file ACME.Backend.RegisterOperation\Dockerfile .
```

