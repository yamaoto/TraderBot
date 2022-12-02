FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TraderBot.EmailListener/TraderBot.EmailListener.csproj", "TraderBot.EmailListener/"]
RUN dotnet restore "TraderBot.EmailListener/TraderBot.EmailListener.csproj"
COPY . .
WORKDIR "/src/TraderBot.EmailListener"
RUN dotnet build "TraderBot.EmailListener.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TraderBot.EmailListener.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TraderBot.EmailListener.dll"]
