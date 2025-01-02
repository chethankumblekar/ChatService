FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 7058

ENV ASPNETCORE_URLS=http://+:7058

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["PlayGround.ChatService/PlayGround.ChatService.Api.csproj", "PlayGround.ChatService/"]
RUN dotnet restore "PlayGround.ChatService/PlayGround.ChatService.Api.csproj"
COPY . .
WORKDIR "/src/PlayGround.ChatService"
RUN dotnet build "PlayGround.ChatService.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "PlayGround.ChatService.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PlayGround.ChatService.Api.dll"]
