#!/bin/bash

[ "$1" = up ] &&
    docker-compose up -d --remove-orphans

[ "$1" = down ] &&
    docker-compose down