FROM alpine:3.20.1

VOLUME [ "/app/source", "/app/target" ]

RUN apk add dotnet8-sdk

WORKDIR /app

COPY . .

WORKDIR /app/FileSync

CMD [ "dotnet", "run", "sync", "-s", "./source", "-t", "-target" ]
