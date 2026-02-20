FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 3001

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src
COPY Soa.Contracts/Soa.Contracts.csproj Soa.Contracts/
COPY Soa.UserService/Soa.UserService.csproj Soa.UserService/
RUN dotnet restore Soa.UserService/Soa.UserService.csproj
COPY Soa.Contracts/ Soa.Contracts/
COPY Soa.UserService/ Soa.UserService/
RUN dotnet publish Soa.UserService/Soa.UserService.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Soa.UserService.dll"]
