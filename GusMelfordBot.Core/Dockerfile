#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0.3-bullseye-slim-arm64v8 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0.201-bullseye-slim-arm64v8 AS build
WORKDIR /src
COPY ["GusMelfordBot.Core/GusMelfordBot.Core.csproj", "GusMelfordBot.Core/"]
COPY ["GusMelfordBot.Database/GusMelfordBot.Database.csproj", "GusMelfordBot.Database/"]
COPY ["GusMelfordBot.DAL/GusMelfordBot.DAL.csproj", "GusMelfordBot.DAL/"]
COPY ["GusMelfordBot.Core.Services/GusMelfordBot.Core.Services.csproj", "GusMelfordBot.Core.Services/"]
COPY ["Telegram.Bot.Core/Telegram.Bot.Core.csproj", "Telegram.Bot.Core/"]
COPY ["Telegram.Api/Telegram.Api.csproj", "Telegram.Api/"]
COPY ["Bot.Api/Bot.Api.csproj", "Bot.Api/"]
COPY ["Telegram.Dto/Telegram.Dto.csproj", "Telegram.Dto/"]
COPY ["Telegram.Exceptions/Telegram.Exceptions.csproj", "Telegram.Exceptions/"]
COPY ["GusMelfordBot.Core.Extensions/GusMelfordBot.Core.Extensions.csproj", "GusMelfordBot.Core.Extensions/"]
COPY ["GusMelfordBot.Core.Domain/GusMelfordBot.Core.Domain.csproj", "GusMelfordBot.Core.Domain/"]
COPY ["GusMelfordBot.Core.CustomExceptions/GusMelfordBot.Core.CustomExceptions.csproj", "GusMelfordBot.Core.CustomExceptions/"]
RUN dotnet restore "GusMelfordBot.Core/GusMelfordBot.Core.csproj"
COPY . .
WORKDIR "/src/GusMelfordBot.Core"
RUN dotnet build "GusMelfordBot.Core.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GusMelfordBot.Core.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GusMelfordBot.Core.dll"]