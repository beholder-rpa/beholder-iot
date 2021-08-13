#!/bin/bash

echo "# Executing Beholder IoT Docker script..."

if [ "$1" = up ]
then
    sudo systemctl enable --now avahi-alias@cerebrum.$HOSTNAME.service
    sudo systemctl enable --now avahi-alias@traefik.$HOSTNAME.service
    sudo systemctl enable --now avahi-alias@nexus.$HOSTNAME.service
    sudo systemctl enable --now avahi-alias@grafana.$HOSTNAME.service

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

    sudo systemctl disable --now avahi-alias@cerebrum.$HOSTNAME.local.service
    sudo systemctl disable --now avahi-alias@traefik.$HOSTNAME.local.service
    sudo systemctl disable --now avahi-alias@nexus.$HOSTNAME.local.service
    sudo systemctl disable --now avahi-alias@grafana.$HOSTNAME.local.service
fi

echo "# Completed Beholder IoT Docker script."