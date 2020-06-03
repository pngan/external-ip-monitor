# external-ip-monitor
Monitors changes to external IP address, and publishes change events.

## Build

Prerequisite is the .Net Core 3.1 SDK.

To build the executable and docker file, run the commands:

    dotnet publish -c Release
    docker build -t counter-image -f Dockerfile .


## Reference
How to build a .Net Core Docker application
https://docs.microsoft.com/en-us/dotnet/core/docker/build-container?tabs=linux    