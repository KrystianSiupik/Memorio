# Images (RD2): one image per host for every environment; configuration comes only from environment
# variables at run time (RD1). Build from the repository root:
#   docker build -t memorio-api .                      # API (the default, last stage)
#   docker build --target worker -t memorio-worker .   # worker (D-036): outbox delivery, module handlers
FROM mcr.microsoft.com/dotnet/sdk:10.0.302 AS build
WORKDIR /src
COPY global.json Directory.Build.props Directory.Packages.props .editorconfig ./
COPY src/ src/
RUN dotnet publish src/Host/Memorio.Api/Memorio.Api.csproj --configuration Release --output /app/api /p:UseAppHost=false \
    && dotnet publish src/Host/Memorio.Worker/Memorio.Worker.csproj --configuration Release --output /app/worker /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0.10 AS base
WORKDIR /app
# Non-root user provided by the .NET images; the host listens on 8080 (ASPNETCORE_HTTP_PORTS).
USER $APP_UID
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

FROM base AS worker
COPY --from=build /app/worker .
ENTRYPOINT ["dotnet", "Memorio.Worker.dll"]

FROM base AS api
COPY --from=build /app/api .
ENTRYPOINT ["dotnet", "Memorio.Api.dll"]
