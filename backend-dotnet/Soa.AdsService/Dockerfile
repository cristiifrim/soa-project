FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 3002

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src
COPY Soa.Contracts/Soa.Contracts.csproj Soa.Contracts/
COPY Soa.AdsService/Soa.AdsService.csproj Soa.AdsService/
RUN dotnet restore Soa.AdsService/Soa.AdsService.csproj
COPY Soa.Contracts/ Soa.Contracts/
COPY Soa.AdsService/ Soa.AdsService/
RUN dotnet publish Soa.AdsService/Soa.AdsService.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Soa.AdsService.dll"]
