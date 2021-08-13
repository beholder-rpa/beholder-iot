#!/bin/bash

echo "# Executing Beholder IoT Docker script..."

if [ "$1" = up ]
then
    export SHORT_HOST=$(echo $HOSTNAME | sed -En 's/^(.*?)\.local$/\1/p')
    sudo systemctl enable --now avahi-alias@cerebrum.$SHORT_HOST.service
    sudo systemctl enable --now avahi-alias@traefik.$SHORT_HOST.service
    sudo systemctl enable --now avahi-alias@nexus.$SHORT_HOST.service
    sudo systemctl enable --now avahi-alias@grafana.$SHORT_HOST.service

    pushd /home/beholder/beholder/
    git pull --depth 1
    ./beholder.ps1 pull rpi
    ./beholder.ps1 build rpi
    ./beholder.ps1 up rpi
    popd
    chown -R pi /home/beholder/beholder/
fi

if [ "$1" = down ]
then
    pushd /home/beholder/beholder/
    ./beholder.ps1 down rpi
    popd

    export SHORT_HOST=$(echo $HOSTNAME | sed -En 's/^(.*?)\.local$/\1/p')
    sudo systemctl disable --now avahi-alias@cerebrum.$SHORT_HOST.service
    sudo systemctl disable --now avahi-alias@traefik.$SHORT_HOST.service
    sudo systemctl disable --now avahi-alias@nexus.$SHORT_HOST.service
    sudo systemctl disable --now avahi-alias@grafana.$SHORT_HOST.service
fi

echo "# Completed Beholder IoT Docker script."