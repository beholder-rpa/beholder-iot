FROM nginx:alpine

COPY nginx.conf /etc/nginx/nginx.conf
COPY api_conf.d /etc/nginx/api_conf.d

EXPOSE 80
ENTRYPOINT ["nginx","-g","daemon off;"]