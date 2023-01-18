#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0.13 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0.405 AS build
WORKDIR /src
COPY ["GusMelfordBot.Api/GusMelfordBot.Api.csproj", "GusMelfordBot.Api/"]
COPY ["GusMelfordBot.Extensions/GusMelfordBot.Extensions.csproj", "GusMelfordBot.Extensions/"]
COPY ["GusMelfordBot.Domain/GusMelfordBot.Domain.csproj", "GusMelfordBot.Domain/"]
COPY ["GusMelfordBot.Infrastructure/GusMelfordBot.Infrastructure.csproj", "GusMelfordBot.Infrastructure/"]
COPY ["GusMelfordBot.SimpleKafka/GusMelfordBot.SimpleKafka.csproj", "GusMelfordBot.SimpleKafka/"]
RUN dotnet restore "GusMelfordBot.Api/GusMelfordBot.Api.csproj"
COPY . .
WORKDIR "/src/GusMelfordBot.Api"
RUN dotnet build "GusMelfordBot.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GusMelfordBot.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GusMelfordBot.Api.dll"]
