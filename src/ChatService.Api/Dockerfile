FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/ChatService.Api/ChatService.Api.csproj",            "src/ChatService.Api/"]
COPY ["src/ChatService.Application/ChatService.Application.csproj", "src/ChatService.Application/"]
COPY ["src/ChatService.Domain/ChatService.Domain.csproj",      "src/ChatService.Domain/"]
COPY ["src/ChatService.Infrastructure/ChatService.Infrastructure.csproj", "src/ChatService.Infrastructure/"]
RUN dotnet restore "src/ChatService.Api/ChatService.Api.csproj"
COPY . .
WORKDIR "/src/src/ChatService.Api"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatService.Api.dll"]
