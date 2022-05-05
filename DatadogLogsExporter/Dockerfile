FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app
RUN mkdir -p /files

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DatadogLogsExporter/DatadogLogsExporter.csproj", "DatadogLogsExporter/"]
RUN dotnet restore "DatadogLogsExporter/DatadogLogsExporter.csproj"
COPY . .
WORKDIR "/src/DatadogLogsExporter"
RUN dotnet build "DatadogLogsExporter.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DatadogLogsExporter.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DatadogLogsExporter.dll"]