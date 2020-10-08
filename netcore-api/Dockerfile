#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["netcore-api/netcore-api.csproj", "netcore-api/"]
COPY ["client.kafka/client.kafka.csproj", "client.kafka/"]
RUN dotnet restore "client.kafka/client.kafka.csproj"
RUN dotnet restore "netcore-api/netcore-api.csproj"
COPY . .
WORKDIR "/src/netcore-api"
RUN dotnet build "netcore-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "netcore-api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "netcore-api.dll"]