FROM node:16-alpine

EXPOSE 4100

VOLUME [ "/app/server/data", "/app/server/assets" ]

RUN apk add openssl

WORKDIR /app

COPY . .

WORKDIR /app/server

RUN npm install

CMD [ "node", "index.js" ]
