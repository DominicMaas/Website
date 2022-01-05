FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Website.csproj", "./"]
RUN dotnet restore "./Website.csproj"
COPY . .
WORKDIR /src/.
RUN dotnet build "Website.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Website.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV AzureKeyVault__ClientId=${{ secrets.AZKV_CLIENT_ID }}
ENV AzureKeyVault__ClientSecret=${{ secrets.AZKV_CLIENT_SECRET }}

ENTRYPOINT ["dotnet", "Website.dll"]