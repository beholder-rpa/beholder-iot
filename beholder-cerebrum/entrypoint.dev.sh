#!/bin/sh
set -ea

if [ ! -d "/usr/src/node-red/data/" ]; then
  echo "creating symbolic link to data folder"
  ln -s /data/ /usr/src/node-red/
fi

if [ ! -f "/usr/src/node-red/data/package.json" ]; then
  echo "./data/package.json not found"
  exit 1
fi

echo "Starting your app..."
CHOKIDAR_USEPOLLING=1
pm2-runtime start /usr/src/node-red/ecosystem.config.dev.yml --delay 5 --max-memory-restart 4G $@