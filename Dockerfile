FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.sln .
COPY src/BRELTV.API/*.csproj ./src/BRELTV.API/
COPY src/BRELTV.Application/*.csproj ./src/BRELTV.Application/
COPY src/BRELTV.Domain/*.csproj ./src/BRELTV.Domain/
COPY src/BRELTV.Infrastructure/*.csproj ./src/BRELTV.Infrastructure/
COPY tests/BRELTV.UnitTests/*.csproj ./tests/BRELTV.UnitTests/
COPY tests/BRELTV.IntegrationTests/*.csproj ./tests/BRELTV.IntegrationTests/

RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet build -c Release --no-restore

# Publish the API
RUN dotnet publish src/BRELTV.API/BRELTV.API.csproj -c Release -o /app/publish --no-build

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "BRELTV.API.dll"]

