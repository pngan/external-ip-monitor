
#!/bin/bash
docker build -t pngan/ipmon .
docker login
docker push pngan/ipmon
docker logout
docker rmi $(docker images -f "dangling=true" -q)
