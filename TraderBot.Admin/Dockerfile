FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TraderBot.Admin/TraderBot.Admin.csproj", "TraderBot.Admin/"]
RUN dotnet restore "TraderBot.Admin/TraderBot.Admin.csproj"
COPY . .
WORKDIR "/src/TraderBot.Admin"
RUN dotnet build "TraderBot.Admin.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TraderBot.Admin.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TraderBot.Admin.dll"]
