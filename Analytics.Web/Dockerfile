FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Analytics.Web/Analytics.Web.csproj", "Analytics.Web/"]
RUN dotnet restore "Analytics.Web/Analytics.Web.csproj"
COPY . .
WORKDIR "/src/Analytics.Web"
RUN dotnet build "Analytics.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Analytics.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Analytics.Web.dll"]
