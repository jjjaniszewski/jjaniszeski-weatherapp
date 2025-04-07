FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["JJaniszewski.WeatherApp.PublicApi/JJaniszewski.WeatherApp.PublicApi.csproj", "JJaniszewski.WeatherApp.PublicApi/"]
RUN dotnet restore "JJaniszewski.WeatherApp.PublicApi/JJaniszewski.WeatherApp.PublicApi.csproj"
COPY . .
WORKDIR "/src/JJaniszewski.WeatherApp.PublicApi"
RUN dotnet build "JJaniszewski.WeatherApp.PublicApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "JJaniszewski.WeatherApp.PublicApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JJaniszewski.WeatherApp.PublicApi.dll"]
