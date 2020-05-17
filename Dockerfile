FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

WORKDIR /
WORKDIR /app

# copy csproj and restore as distinct layers
COPY src/*.sln ./src/
COPY src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done
WORKDIR /app/src
RUN dotnet restore

# copy and publish app and libraries
WORKDIR /app
COPY ./src ./src
WORKDIR /app/src/VehiclesCostMonitoring.Bot.Server
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS runtime
COPY --from=build /out ./
ENTRYPOINT ["dotnet", "VehiclesCostMonitoring.Bot.Server.dll"]