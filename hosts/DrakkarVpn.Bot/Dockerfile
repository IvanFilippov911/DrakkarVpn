FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY DrakkarVpn.sln ./
COPY Directory.Packages.props ./
COPY hosts/DrakkarVpn.Bot/DrakkarVpn.Bot.csproj hosts/DrakkarVpn.Bot/

RUN dotnet restore hosts/DrakkarVpn.Bot/DrakkarVpn.Bot.csproj

COPY . .

RUN dotnet publish hosts/DrakkarVpn.Bot/DrakkarVpn.Bot.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "DrakkarVpn.Bot.dll"]