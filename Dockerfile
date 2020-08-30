FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /

COPY . /

WORKDIR /App
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "NetCore.Docker.dll"]
