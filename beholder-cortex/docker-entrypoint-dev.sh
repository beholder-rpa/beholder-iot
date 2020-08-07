#!/bin/sh
set -ea

if [ ! -f "package.json" ]; then

    echo "package.json not found"
    exit 1
fi

echo "Ensuring dependencies..."
yarn global add nodemon
yarn install

echo "Starting your app..."
nodemon -w package.json --delay 10 --exec "yarn && $@"