FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TraderBot.BinanceConnect/TraderBot.BinanceConnect.csproj", "TraderBot.BinanceConnect/"]
COPY ["TraderBot.Abstractions/TraderBot.Abstractions.csproj", "TraderBot.Abstractions/TraderBot.Abstractions.csproj"]
RUN dotnet restore "TraderBot.BinanceConnect/TraderBot.BinanceConnect.csproj"
COPY . .
WORKDIR "/src/TraderBot.BinanceConnect"
# RUN dotnet build "TraderBot.BinanceConnect.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TraderBot.BinanceConnect.csproj" -c Release -o /app/publish -r linux-x64 -p:PublishSingleFile=true --self-contained

FROM mcr.microsoft.com/dotnet/nightly/runtime-deps:7.0.0-jammy-chiseled-amd64 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["/app/TraderBot.BinanceConnect"]
