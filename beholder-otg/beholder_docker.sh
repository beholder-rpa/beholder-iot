#!/bin/bash

echo "# Executing Beholder IoT Docker script..."

[ "$1" = up ] &&
    docker-compose up -d --remove-orphans

[ "$1" = down ] &&
    docker-compose down

echo "# Completed Beholder IoT Docker script."