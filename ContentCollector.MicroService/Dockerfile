FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ContentCollector/ContentCollector.csproj", "ContentCollector/"]
RUN dotnet restore "ContentCollector/ContentCollector.csproj"
COPY . .
WORKDIR "/src/ContentCollector"
RUN dotnet build "ContentCollector.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ContentCollector.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContentCollector.dll"]
