FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TraderBot.OrderController/TraderBot.OrderController.csproj", "TraderBot.OrderController/"]
RUN dotnet restore "TraderBot.OrderController/TraderBot.OrderController.csproj"
COPY . .
WORKDIR "/src/TraderBot.OrderController"
RUN dotnet build "TraderBot.OrderController.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TraderBot.OrderController.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TraderBot.OrderController.dll"]
