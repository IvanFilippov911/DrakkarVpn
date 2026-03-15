FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY DrakkarVpn.sln ./
COPY Directory.Packages.props ./

COPY hosts/DrakkarVpn.Admin.Api/DrakkarVpn.Admin.Api.csproj hosts/DrakkarVpn.Admin.Api/
COPY infrastructure/DrakkarVpn.HostInfrastructure/DrakkarVpn.HostInfrastructure.csproj infrastructure/DrakkarVpn.HostInfrastructure/
COPY infrastructure/DrakkarVpn.Idempotency/DrakkarVpn.Idempotency.csproj infrastructure/DrakkarVpn.Idempotency/
COPY infrastructure/DrakkarVpn.Shared/DrakkarVpn.Shared.csproj infrastructure/DrakkarVpn.Shared/
COPY modules/DrakkarVpn.AdminAuth/DrakkarVpn.AdminAuth.csproj modules/DrakkarVpn.AdminAuth/
COPY modules/DrakkarVpn.Observability/DrakkarVpn.Observability.csproj modules/DrakkarVpn.Observability/
COPY modules/DrakkarVpn.Peers/DrakkarVpn.Peers.csproj modules/DrakkarVpn.Peers/
COPY modules/DrakkarVpn.Servers/DrakkarVpn.Servers.csproj modules/DrakkarVpn.Servers/
COPY modules/DrakkarVpn.Subscriptions/DrakkarVpn.Subscriptions.csproj modules/DrakkarVpn.Subscriptions/
COPY modules/DrakkarVpn.Tariffs/DrakkarVpn.Tariffs.csproj modules/DrakkarVpn.Tariffs/
COPY modules/DrakkarVpn.Users/DrakkarVpn.Users.csproj modules/DrakkarVpn.Users/

RUN dotnet restore hosts/DrakkarVpn.Admin.Api/DrakkarVpn.Admin.Api.csproj

COPY . .

RUN dotnet publish hosts/DrakkarVpn.Admin.Api/DrakkarVpn.Admin.Api.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "DrakkarVpn.Admin.Api.dll"]