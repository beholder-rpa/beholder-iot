#!/bin/bash

echo "# Executing Beholder IoT Docker script..."

if [ "$1" = up ]
then
    pushd /home/beholder/beholder/
    ./beholder.ps1 build rpi
    ./beholder.ps1 up rpi
    popd
fi

if [ "$1" = down ]
then
    pushd /home/beholder/beholder/
    ./beholder.ps1 down rpi
    popd
fi

echo "# Completed Beholder IoT Docker script."