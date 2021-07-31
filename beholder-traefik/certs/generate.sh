#!/usr/bin/sh
openssl req -x509 -out localhost.crt -keyout localhost.key \
  -days 9999 \
  -newkey rsa:2048 -nodes -sha256 \
  -subj '/CN=localhost'