# ── Build stage ───────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/GoodHamburger.Core/GoodHamburger.Core.csproj", "src/GoodHamburger.Core/"]
COPY ["src/GoodHamburger.Blazor/GoodHamburger.Blazor.csproj", "src/GoodHamburger.Blazor/"]

RUN dotnet restore "src/GoodHamburger.Blazor/GoodHamburger.Blazor.csproj"

COPY . .

RUN dotnet publish "src/GoodHamburger.Blazor/GoodHamburger.Blazor.csproj" \
    -c Release -o /app/publish --no-restore

# ── Runtime stage — nginx para servir arquivos estáticos ──────────────────────
FROM nginx:alpine AS runtime

# Remove config padrão do nginx
RUN rm /etc/nginx/conf.d/default.conf

COPY --from=build /app/publish/wwwroot /usr/share/nginx/html

# Config nginx para Blazor WASM (SPA routing)
COPY src/GoodHamburger.Blazor/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
