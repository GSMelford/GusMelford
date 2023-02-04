#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0.13 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0.405 AS build
WORKDIR /src
COPY ["ContentProcessor.Worker/ContentProcessor.Worker.csproj", "ContentCollector.MicroService/"]
COPY ["GusMelfordBot.Extensions/GusMelfordBot.Extensions.csproj", "GusMelfordBot.Extensions/"]
COPY ["GusMelfordBot.SimpleKafka/GusMelfordBot.SimpleKafka.csproj", "GusMelfordBot.SimpleKafka/"]
RUN dotnet restore "ContentProcessor.Worker/ContentProcessor.Worker.csproj"
COPY . .
WORKDIR "/src/ContentProcessor.Worker"
RUN dotnet build "ContentProcessor.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ContentProcessor.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContentProcessor.Worker.dll"]
