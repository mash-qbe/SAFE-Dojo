FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

COPY . .

RUN dotnet tool restore
RUN dotnet tool install paket --tool-path /app/tools
ENV PATH="${PATH}:/app/tools"
RUN dotnet tool run paket restore

COPY src/Client/Client.fsproj src/Client/
RUN dotnet restore src/Client/Client.fsproj

COPY . .
RUN dotnet publish src/Server/Server.fsproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
RUN apt-get update && apt-get install -y net-tools

EXPOSE 8080
ENTRYPOINT ["dotnet", "Server.dll"]
