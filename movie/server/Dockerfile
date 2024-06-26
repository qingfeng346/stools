FROM node:16-alpine

EXPOSE 4200

VOLUME [ "/app/data", "/app/assets" ]

RUN apk add openssl

WORKDIR /app

COPY . .

WORKDIR /app

RUN npm install

CMD [ "node", "index.js" ]
