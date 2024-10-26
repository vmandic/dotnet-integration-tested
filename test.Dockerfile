# Base image for .NET SDK 7.0.203
FROM mcr.microsoft.com/dotnet/sdk:7.0.203 AS build-env

# Install Node.js, npm, and Yarn
RUN apt-get update && \
    apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_16.x | bash - && \
    apt-get install -y nodejs && \
    npm install --global yarn && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

# Set working directory
WORKDIR /app

# Copy the solution file and props files
COPY global.json ./
COPY DotnetIntegrationTested.sln ./

# Copy each project directory individually to preserve structure
COPY src/DotnetIntegrationTested.AuthApi/DotnetIntegrationTested.AuthApi.csproj src/DotnetIntegrationTested.AuthApi/DotnetIntegrationTested.AuthApi.csproj
COPY src/DotnetIntegrationTested.Common/DotnetIntegrationTested.Common.csproj src/DotnetIntegrationTested.Common/DotnetIntegrationTested.Common.csproj
COPY src/DotnetIntegrationTested.ExternalApis/DotnetIntegrationTested.ExternalApis.csproj src/DotnetIntegrationTested.ExternalApis/DotnetIntegrationTested.ExternalApis.csproj
COPY src/DotnetIntegrationTested.HttpApi/DotnetIntegrationTested.HttpApi.csproj src/DotnetIntegrationTested.HttpApi/DotnetIntegrationTested.HttpApi.csproj
COPY src/DotnetIntegrationTested.Services/DotnetIntegrationTested.Services.csproj src/DotnetIntegrationTested.Services/DotnetIntegrationTested.Services.csproj
COPY src/DotnetIntegrationTested.SqlMigrations/DotnetIntegrationTested.SqlMigrations.csproj src/DotnetIntegrationTested.SqlMigrations/DotnetIntegrationTested.SqlMigrations.csproj
COPY src/DotnetIntegrationTested.Worker/DotnetIntegrationTested.Worker.csproj src/DotnetIntegrationTested.Worker/DotnetIntegrationTested.Worker.csproj
COPY tests/DotnetIntegrationTested.IntegrationTests/DotnetIntegrationTested.IntegrationTests.csproj tests/DotnetIntegrationTested.IntegrationTests/DotnetIntegrationTested.IntegrationTests.csproj
COPY tests/DotnetIntegrationTested.UnitTests/DotnetIntegrationTested.UnitTests.csproj tests/DotnetIntegrationTested.UnitTests/DotnetIntegrationTested.UnitTests.csproj

# Perform a restore
RUN dotnet restore DotnetIntegrationTested.sln

# Copy the rest of the source code
COPY . .

# Copy docker/conf directory to maintain relative path
COPY src/docker/conf /app/src/docker/conf

# Build the solution
RUN dotnet build DotnetIntegrationTested.sln -c Release

ENV DOTNET_ENVIRONMENT=IntegrationTests

# Default to a basic test command (can be overridden in the CI/CD pipeline)
CMD ["dotnet", "test", "-c", "Release", "--no-build", "--logger:trx"]
