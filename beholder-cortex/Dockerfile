FROM node:12-alpine

WORKDIR /srv/app

COPY ./package.json ./
COPY ./yarn.lock ./

RUN yarn install

COPY . .

ENV NODE_ENV production

EXPOSE 3000

RUN yarn build

ENTRYPOINT ["/usr/local/bin/yarn", "start"]
