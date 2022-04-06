#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:6.0.3-bullseye-slim-arm64v8 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0.201-bullseye-slim-arm64v8 AS build
WORKDIR /src
COPY ["GusMelfordBot.Core/GusMelfordBot.Core.csproj", "GusMelfordBot.Core/"]
COPY ["GusMelfordBot.Database/GusMelfordBot.Database.csproj", "GusMelfordBot.Database/"]
COPY ["GusMelfordBot.DAL/GusMelfordBot.DAL.csproj", "GusMelfordBot.DAL/"]
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