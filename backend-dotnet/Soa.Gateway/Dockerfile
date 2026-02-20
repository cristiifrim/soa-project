FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 3000

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src
COPY Soa.Contracts/Soa.Contracts.csproj Soa.Contracts/
COPY Soa.Gateway/Soa.Gateway.csproj Soa.Gateway/
RUN dotnet restore Soa.Gateway/Soa.Gateway.csproj
COPY Soa.Contracts/ Soa.Contracts/
COPY Soa.Gateway/ Soa.Gateway/
RUN dotnet publish Soa.Gateway/Soa.Gateway.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Soa.Gateway.dll"]
