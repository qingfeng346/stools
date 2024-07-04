FROM node:16-alpine

EXPOSE 4200

VOLUME [ "/app/data", "/app/assets/media", "/app/assets/cache" ]

RUN apk add openssl

WORKDIR /app

COPY . .

WORKDIR /app

RUN npm install

CMD [ "node", "index.js" ]
