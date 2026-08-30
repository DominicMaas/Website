# ---- build ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore against the target RID so the publish below can prune every other
# runtime. Without this the output carries libSkiaSharp for ~25 platforms.
COPY ["Website.csproj", "./"]
RUN dotnet restore "Website.csproj" -r linux-x64

COPY . .
RUN dotnet publish "Website.csproj" \
    -c Release \
    -r linux-x64 \
    --self-contained false \
    --no-restore \
    -p:DebugType=none \
    -p:DebugSymbols=false \
    -o /app/publish \
 && mkdir -p /app/static \
 && mv /app/publish/wwwroot /app/static/wwwroot

# ---- runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

# Static assets and application code ship as separate layers so neither blob
# grows near the 100MB per-request cap Cloudflare puts in front of the registry.
COPY --from=build /app/static/wwwroot ./wwwroot
COPY --from=build /app/publish/ ./

ENTRYPOINT ["dotnet", "Website.dll"]
