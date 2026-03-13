FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY DrakkarVpn.sln ./
COPY Directory.Packages.props ./

COPY hosts/DrakkarVpn.DbMigrator/DrakkarVpn.DbMigrator.csproj hosts/DrakkarVpn.DbMigrator/

COPY infrastructure/DrakkarVpn.Idempotency/DrakkarVpn.Idempotency.csproj infrastructure/DrakkarVpn.Idempotency/
COPY modules/DrakkarVpn.Users/DrakkarVpn.Users.csproj modules/DrakkarVpn.Users/
COPY modules/DrakkarVpn.Peers/DrakkarVpn.Peers.csproj modules/DrakkarVpn.Peers/
COPY modules/DrakkarVpn.Servers/DrakkarVpn.Servers.csproj modules/DrakkarVpn.Servers/
COPY modules/DrakkarVpn.Subscriptions/DrakkarVpn.Subscriptions.csproj modules/DrakkarVpn.Subscriptions/
COPY modules/DrakkarVpn.Tariffs/DrakkarVpn.Tariffs.csproj modules/DrakkarVpn.Tariffs/
COPY modules/DrakkarVpn.Observability/DrakkarVpn.Observability.csproj modules/DrakkarVpn.Observability/

RUN dotnet restore hosts/DrakkarVpn.DbMigrator/DrakkarVpn.DbMigrator.csproj

COPY . .

RUN dotnet publish hosts/DrakkarVpn.DbMigrator/DrakkarVpn.DbMigrator.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "DrakkarVpn.DbMigrator.dll"]