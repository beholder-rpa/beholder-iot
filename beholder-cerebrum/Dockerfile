FROM nodered/node-red:latest

WORKDIR /data-seed
COPY ./data .
RUN npm install

WORKDIR /usr/src/node-red
COPY ./entrypoint.sh .
ENTRYPOINT ["/usr/src/node-red/entrypoint.sh"]