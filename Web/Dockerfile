FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["WebFiber/WebFiber.csproj", "WebFiber/"]
COPY ["ApplicationFiber/ApplicationFiber.csproj", "ApplicationFiber/"]
COPY ["CoreLayer/CoreLayer.csproj", "CoreLayer/"]
COPY ["DomainFiber/DomainFiber.csproj", "DomainFiber/"]
COPY ["Utils/Utils.csproj", "Utils/"]
RUN dotnet restore "WebFiber/WebFiber.csproj"
COPY . .
WORKDIR "/src/WebFiber"
RUN dotnet build "WebFiber.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebFiber.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebFiber.dll"]