#!/bin/bash

echo "# Executing Beholder IoT Docker script..."

if [ "$1" = up ]
then
    ~/beholder/beholder.ps1 build rpi
    ~/beholder/beholder.ps1 up rpi
fi

if [ "$1" = down ]
then
    ~/beholder/beholder.ps1 down rpi
fi

echo "# Completed Beholder IoT Docker script."