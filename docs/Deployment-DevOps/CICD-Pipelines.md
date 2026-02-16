# CI/CD Pipelines

> Subject: [Deployment-DevOps](../README.md)

## CI/CD Pipelines

### GitHub Actions

```yaml
# .github/workflows/deploy.yml
name: Build and Deploy

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

env:
  REGISTRY: myregistry.azurecr.io
  IMAGE_NAME: myapp

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: "10.0.x"

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --configuration Release --no-restore

      - name: Test
        run: dotnet test --no-build --verbosity normal

  docker-build-push:
    needs: build-and-test
    runs-on: ubuntu-latest
    if: github.event_name == 'push'
    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Login to Azure Container Registry
        uses: docker/login-action@v3
        with:
          registry: ${{ env.REGISTRY }}
          username: ${{ secrets.ACR_USERNAME }}
          password: ${{ secrets.ACR_PASSWORD }}

      - name: Extract metadata
        id: meta
        uses: docker/metadata-action@v5
        with:
          images: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}
          tags: |
            type=sha,prefix={{branch}}-
            type=semver,pattern={{version}}

      - name: Build and push
        uses: docker/build-push-action@v5
        with:
          context: .
          push: true
          tags: ${{ steps.meta.outputs.tags }}
          labels: ${{ steps.meta.outputs.labels }}

  deploy-to-kubernetes:
    needs: docker-build-push
    runs-on: ubuntu-latest
    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Setup kubectl
        uses: azure/setup-kubectl@v3

      - name: Set Kubernetes context
        uses: azure/k8s-set-context@v3
        with:
          kubeconfig: ${{ secrets.KUBE_CONFIG }}

      - name: Deploy to Kubernetes
        run: |
          kubectl apply -f k8s/deployment.yaml
          kubectl apply -f k8s/service.yaml
          kubectl rollout status deployment/myapp-deployment
```

### Azure DevOps Pipeline

```yaml
# azure-pipelines.yml
trigger:
  branches:
    include:
      - main

pool:
  vmImage: "ubuntu-latest"

variables:
  buildConfiguration: "Release"
  dockerRegistryServiceConnection: "MyACR"
  imageRepository: "myapp"
  containerRegistry: "myregistry.azurecr.io"
  tag: "$(Build.BuildId)"

stages:
  - stage: Build
    jobs:
      - job: BuildAndTest
        steps:
          - task: UseDotNet@2
            inputs:
              version: "10.0.x"

          - task: DotNetCoreCLI@2
            displayName: "Restore"
            inputs:
              command: "restore"

          - task: DotNetCoreCLI@2
            displayName: "Build"
            inputs:
              command: "build"
              arguments: "--configuration $(buildConfiguration)"

          - task: DotNetCoreCLI@2
            displayName: "Test"
            inputs:
              command: "test"
              arguments: "--configuration $(buildConfiguration) --no-build"

  - stage: Docker
    dependsOn: Build
    jobs:
      - job: BuildPushImage
        steps:
          - task: Docker@2
            displayName: "Build and Push"
            inputs:
              containerRegistry: "$(dockerRegistryServiceConnection)"
              repository: "$(imageRepository)"
              command: "buildAndPush"
              Dockerfile: "**/Dockerfile"
              tags: |
                $(tag)
                latest

  - stage: Deploy
    dependsOn: Docker
    jobs:
      - deployment: DeployToK8s
        environment: "production"
        strategy:
          runOnce:
            deploy:
              steps:
                - task: KubernetesManifest@0
                  inputs:
                    action: "deploy"
                    manifests: |
                      k8s/deployment.yaml
                      k8s/service.yaml
                    containers: "$(containerRegistry)/$(imageRepository):$(tag)"
```

---


