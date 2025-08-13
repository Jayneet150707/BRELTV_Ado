FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/BRELTV.API/BRELTV.API.csproj", "BRELTV.API/"]
COPY ["src/BRELTV.DataAccess/BRELTV.DataAccess.csproj", "BRELTV.DataAccess/"]
COPY ["src/BRELTV.Models/BRELTV.Models.csproj", "BRELTV.Models/"]
COPY ["src/BRELTV.Services/BRELTV.Services.csproj", "BRELTV.Services/"]
RUN dotnet restore "BRELTV.API/BRELTV.API.csproj"
COPY src/ .
WORKDIR "/src/BRELTV.API"
RUN dotnet build "BRELTV.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BRELTV.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BRELTV.API.dll"]

