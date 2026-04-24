# ── Build stage ───────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/GoodHamburger.Core/GoodHamburger.Core.csproj", "src/GoodHamburger.Core/"]
COPY ["src/GoodHamburger.Infrastructure/GoodHamburger.Infrastructure.csproj", "src/GoodHamburger.Infrastructure/"]
COPY ["src/GoodHamburger.Api/GoodHamburger.Api.csproj", "src/GoodHamburger.Api/"]

RUN dotnet restore "src/GoodHamburger.Api/GoodHamburger.Api.csproj"

COPY . .

RUN dotnet publish "src/GoodHamburger.Api/GoodHamburger.Api.csproj" \
    -c Release -o /app/publish --no-restore

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Usuário não-root — OWASP A05
RUN adduser --disabled-password --gecos "" appuser
USER appuser

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "GoodHamburger.Api.dll"]
