# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY RealtimeChat.slnx ./
COPY RealtimeChat.Api/*.csproj ./RealtimeChat.Api/
COPY RealtimeChat.BLL/*.csproj ./RealtimeChat.BLL/
COPY RealtimeChat.DAL/*.csproj ./RealtimeChat.DAL/

RUN dotnet restore RealtimeChat.Api/RealtimeChat.Api.csproj

COPY . .

RUN dotnet publish RealtimeChat.Api/RealtimeChat.Api.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "RealtimeChat.Api.dll"]