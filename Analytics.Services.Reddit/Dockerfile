FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Analytics.Services.Reddit/Analytics.Services.Reddit.csproj", "Analytics.Services.Reddit/"]
RUN dotnet restore "Analytics.Services.Reddit/Analytics.Services.Reddit.csproj"
COPY . .
WORKDIR "/src/Analytics.Services.Reddit"
RUN dotnet build "Analytics.Services.Reddit.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Analytics.Services.Reddit.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Analytics.Services.Reddit.dll"]
