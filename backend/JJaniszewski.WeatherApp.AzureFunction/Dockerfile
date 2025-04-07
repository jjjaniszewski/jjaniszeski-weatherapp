FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["JJaniszewski.WeatherApp.AzureFunction/JJaniszewski.WeatherApp.AzureFunction.csproj", "JJaniszewski.WeatherApp.AzureFunction/"]
COPY ["JJaniszewski.WeatherApp.Infrastructure/JJaniszewski.WeatherApp.Infrastructure.csproj", "JJaniszewski.WeatherApp.Infrastructure/"]
COPY ["JJaniszewski.WeatherApp.Application/JJaniszewski.WeatherApp.Application.csproj", "JJaniszewski.WeatherApp.Application/"]
COPY ["JJaniszewski.WeatherApp.Domain/JJaniszewski.WeatherApp.Domain.csproj", "JJaniszewski.WeatherApp.Domain/"]
RUN dotnet restore "JJaniszewski.WeatherApp.AzureFunction/JJaniszewski.WeatherApp.AzureFunction.csproj"
COPY . .
WORKDIR "/src/JJaniszewski.WeatherApp.AzureFunction"
RUN dotnet build "JJaniszewski.WeatherApp.AzureFunction.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "JJaniszewski.WeatherApp.AzureFunction.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JJaniszewski.WeatherApp.AzureFunction.dll"]
