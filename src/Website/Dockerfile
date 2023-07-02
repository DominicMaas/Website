FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src/Website
COPY ["Website.csproj", "./"]
RUN dotnet restore "./Website.csproj"
COPY . .
WORKDIR /src/Website/.
RUN dotnet build "Website.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Website.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Website.dll"]