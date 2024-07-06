# external-ip-monitor
Monitors changes to external IP address. If the external IP has changed for a A Record specified in an 
environment variable `DNSARECORD` (e.g. `example.com`), then the address for that DNS A Record is changed at the OVH registry.

Login credentials to Ovh are stored in a file called `ovh.conf` located in the login directory of the user. This can be retrieved
from example code from ovh .net client.

# Build and run executable

Prerequisite is the .Net Core 8.0 SDK, which must be installed on the system.

    dotnet build
    cd App
    dotnet run

# Build and run tests

    cd tests/ipchange-detector-tests
    dotnet test
    cd ../ipchange-detector-tests
    dotnet test

## Build and run docker container

Prerequisite is Docker.

To build the executable and docker file, run the commands:

    .\build.cmd


To run on home server

    .\deploy-ipmon


To run from command line

    docker volume create ipmon-vol  # Create volume to store old ip address
    docker run -v ipmon-vol:/ipmon pngan/ipmon

## Reference
How to build a .Net Core Docker application
https://stackify.com/a-start-to-finish-guide-to-docker-for-net/
https://docs.microsoft.com/en-us/dotnet/core/docker/build-container?tabs=linux    