docker build -t itrofimow/achiever-api:latest -f build/Dockerfile .
docker push itrofimow/achiever-api

# remote docker rm -f $(docker ps -a -f ancestor=itrofimow/achiever-api -q)
