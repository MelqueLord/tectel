# Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY src/AssistenciaTecnica.Domain/AssistenciaTecnica.Domain.csproj src/AssistenciaTecnica.Domain/
COPY src/AssistenciaTecnica.Application/AssistenciaTecnica.Application.csproj src/AssistenciaTecnica.Application/
COPY src/AssistenciaTecnica.Infrastructure/AssistenciaTecnica.Infrastructure.csproj src/AssistenciaTecnica.Infrastructure/
COPY src/AssistenciaTecnica.Web/AssistenciaTecnica.Web.csproj src/AssistenciaTecnica.Web/

RUN dotnet restore src/AssistenciaTecnica.Web/AssistenciaTecnica.Web.csproj

# Copy everything else and build
COPY src/ src/
WORKDIR /src/src/AssistenciaTecnica.Web
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Install icu for globalization
RUN apt-get update && apt-get install -y libicu-dev && rm -rf /var/lib/apt/lists/*

# Create logs directory
RUN mkdir -p /app/logs

# Copy published app
COPY --from=build /app/publish .

# Create non-root user
RUN useradd -m -u 1000 appuser && chown -R appuser:appuser /app
USER appuser

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

ENTRYPOINT ["dotnet", "AssistenciaTecnica.Web.dll"]
