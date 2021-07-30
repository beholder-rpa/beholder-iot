#!/bin/bash

echo "# Executing Beholder IoT Docker script..."

if [ "$1" = up ]
then
    ./beholder.ps1 build prod
    ./beholder.ps1 up prod
fi

if [ "$1" = down ]
then
    ./beholder.ps1 down prod
fi

echo "# Completed Beholder IoT Docker script."