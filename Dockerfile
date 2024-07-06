FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /

COPY . /

WORKDIR /App
RUN dotnet clean --configuration Release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "NetCore.Docker.dll"]
