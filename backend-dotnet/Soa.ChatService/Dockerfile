FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 3003

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src
COPY Soa.Contracts/Soa.Contracts.csproj Soa.Contracts/
COPY Soa.ChatService/Soa.ChatService.csproj Soa.ChatService/
RUN dotnet restore Soa.ChatService/Soa.ChatService.csproj
COPY Soa.Contracts/ Soa.Contracts/
COPY Soa.ChatService/ Soa.ChatService/
RUN dotnet publish Soa.ChatService/Soa.ChatService.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Soa.ChatService.dll"]
