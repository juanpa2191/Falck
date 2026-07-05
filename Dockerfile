# syntax=docker/dockerfile:1

# ---- Etapa de build ----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia primero la solución y los archivos de proyecto para que "dotnet restore"
# quede cacheado y solo se vuelva a ejecutar cuando cambie un .csproj (no en cada
# edición de código fuente).
COPY Falck.sln nuget.config ./
COPY src/Falck.Domain/Falck.Domain.csproj         src/Falck.Domain/
COPY src/Falck.Application/Falck.Application.csproj src/Falck.Application/
COPY src/Falck.Infrastructure/Falck.Infrastructure.csproj src/Falck.Infrastructure/
COPY src/Falck.Api/Falck.Api.csproj               src/Falck.Api/
RUN dotnet restore src/Falck.Api/Falck.Api.csproj

# Copia el resto del código fuente y publica un build en Release.
COPY src/ src/
RUN dotnet publish src/Falck.Api/Falck.Api.csproj -c Release -o /app --no-restore

# ---- Etapa de runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app ./

# Kestrel escucha en el 8080 (amigable con no-root, el valor por defecto del contenedor .NET).
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Falck.Api.dll"]
