FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY DrakkarVpn.sln ./
COPY Directory.Packages.props ./

COPY hosts/DrakkarVpn.Agent/DrakkarVpn.Agent.csproj hosts/DrakkarVpn.Agent/
COPY infrastructure/DrakkarVpn.Shared/DrakkarVpn.Shared.csproj infrastructure/DrakkarVpn.Shared/

RUN dotnet restore hosts/DrakkarVpn.Agent/DrakkarVpn.Agent.csproj

COPY . .

RUN dotnet publish hosts/DrakkarVpn.Agent/DrakkarVpn.Agent.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN adduser --disabled-password --home /app --gecos "" appuser
USER appuser

COPY --from=build /app/publish .

EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5046
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "DrakkarVpn.Agent.dll"]