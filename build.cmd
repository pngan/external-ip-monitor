
rem echo off

docker build -t pngan/ipmon .
docker login
docker push pngan/ipmon
docker logout


echo on
