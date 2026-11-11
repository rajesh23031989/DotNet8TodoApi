# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["DotNet8TodoApi.sln", "."]
COPY ["src/API/API.csproj", "src/API/"]
COPY ["src/Metadata/Metadata.csproj", "src/Metadata/"]
COPY ["src/Data/Data.csproj", "src/Data/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["tests/IntegrationTest/IntegrationTest.csproj", "tests/IntegrationTest/"]
RUN dotnet restore "DotNet8TodoApi.sln"

COPY . .
RUN dotnet test "DotNet8TodoApi.sln" -c Release --no-restore
RUN dotnet publish "src/API/API.csproj" -c Release --no-restore -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_EnableDiagnostics=0
EXPOSE 8080

COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "API.dll"]
