FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TraderBot.BinanceConnect/TraderBot.BinanceConnect.csproj", "TraderBot.BinanceConnect/"]
RUN dotnet restore "TraderBot.BinanceConnect/TraderBot.BinanceConnect.csproj"
COPY . .
WORKDIR "/src/TraderBot.BinanceConnect"
RUN dotnet build "TraderBot.BinanceConnect.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TraderBot.BinanceConnect.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TraderBot.BinanceConnect.dll"]
