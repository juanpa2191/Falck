# syntax=docker/dockerfile:1

# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution + project files first so "dotnet restore" is cached and only
# re-runs when a .csproj changes (not on every source edit).
COPY Falck.sln nuget.config ./
COPY src/Falck.Domain/Falck.Domain.csproj         src/Falck.Domain/
COPY src/Falck.Application/Falck.Application.csproj src/Falck.Application/
COPY src/Falck.Infrastructure/Falck.Infrastructure.csproj src/Falck.Infrastructure/
COPY src/Falck.Api/Falck.Api.csproj               src/Falck.Api/
RUN dotnet restore src/Falck.Api/Falck.Api.csproj

# Copy the rest of the source and publish a trimmed, Release build.
COPY src/ src/
RUN dotnet publish src/Falck.Api/Falck.Api.csproj -c Release -o /app --no-restore

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app ./

# Kestrel listens on 8080 (non-root friendly, the .NET container default).
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Falck.Api.dll"]
