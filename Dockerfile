FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /app

COPY . .

RUN dotnet restore ./Build.fsproj

RUN dotnet build ./Build.fsproj -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime

WORKDIR /app

COPY --from=build /app/build .

EXPOSE 80

ENTRYPOINT ["dotnet", "Server.dll"]
