# external-ip-monitor
Monitors changes to external IP address, and publishes change events.

# Build and run executable

Prerequisite is the .Net Core 3.1 SDK, which must be installed on the system.

    cd App
    dotnet build
    dotnet run

## Build and run docker container

Prerequisite is Docker.

To build the executable and docker file, run the commands:

    cd App
    docker build -t pngan/ipmon .

To run
    docker volume create ipmon-vol
    docker run -v ipmon-vol:/ipmon pngan/ipmon


## Reference
How to build a .Net Core Docker application
https://stackify.com/a-start-to-finish-guide-to-docker-for-net/
https://docs.microsoft.com/en-us/dotnet/core/docker/build-container?tabs=linux    