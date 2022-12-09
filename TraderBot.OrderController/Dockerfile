FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TraderBot.OrderController/TraderBot.OrderController.csproj", "TraderBot.OrderController/"]
COPY ["TraderBot.Abstractions/TraderBot.Abstractions.csproj", "TraderBot.Abstractions/TraderBot.Abstractions.csproj"]
COPY ["TraderBot.RavenDb/TraderBot.RavenDb.csproj", "TraderBot.RavenDb/TraderBot.RavenDb.csproj"]
RUN dotnet restore "TraderBot.OrderController/TraderBot.OrderController.csproj"
COPY . .
WORKDIR "/src/TraderBot.OrderController"
# RUN dotnet build "TraderBot.OrderController.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TraderBot.OrderController.csproj" -c Release -o /app/publish -r linux-x64 -p:PublishSingleFile=true --self-contained

FROM mcr.microsoft.com/dotnet/nightly/runtime-deps:7.0.0-jammy-chiseled-amd64 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["/app/TraderBot.OrderController"]