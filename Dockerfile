# API image (RD2): one image for every environment; configuration comes only from environment
# variables at run time (RD1), e.g. -e AppVersion__Minimum=0.1.0. Build from the repository root:
#   docker build -t memorio-api .
FROM mcr.microsoft.com/dotnet/sdk:10.0.302 AS build
WORKDIR /src
COPY global.json Directory.Build.props Directory.Packages.props .editorconfig ./
COPY src/ src/
RUN dotnet publish src/Host/Memorio.Api/Memorio.Api.csproj --configuration Release --output /app /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0.10 AS runtime
WORKDIR /app
COPY --from=build /app .
# Non-root user provided by the .NET images; the API listens on 8080 (ASPNETCORE_HTTP_PORTS).
USER $APP_UID
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Memorio.Api.dll"]
