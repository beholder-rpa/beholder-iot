#!/bin/bash

echo "# Executing Beholder IoT Docker script..."

if [ "$1" = up ]
then
    yarn
    yarn prod-up
fi

if [ "$1" = down ]
then
    yarn prod-down
fi

echo "# Completed Beholder IoT Docker script."