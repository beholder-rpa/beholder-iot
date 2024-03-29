#!/bin/bash

echo "# Executing Beholder IoT Docker script..."

if [ "$1" = up ]
then
    #export SHORT_HOST=$(echo $HOSTNAME | sed -En 's/^(.*?)\.local$/\1/p')
    sudo systemctl enable --now avahi-alias@"cerebrum.$HOSTNAME".service
    sudo systemctl enable --now avahi-alias@"nexus.$HOSTNAME".service
    sudo systemctl enable --now avahi-alias@"grafana.$HOSTNAME".service
    sudo systemctl enable --now avahi-alias@"minio.$HOSTNAME".service

    pushd /home/beholder/beholder/
    git reset origin --hard
    git pull --depth 1
    ./beholder.ps1 pull
    ./beholder.ps1 build
    ./beholder.ps1 up
    popd
    chown -R pi /home/beholder/beholder/
fi

if [ "$1" = down ]
then
    pushd /home/beholder/beholder/
    ./beholder.ps1 down
    popd

    #export SHORT_HOST=$(echo $HOSTNAME | sed -En 's/^(.*?)\.local$/\1/p')
    sudo systemctl disable --now avahi-alias@"cerebrum.$HOSTNAME".service
    sudo systemctl disable --now avahi-alias@"nexus.$HOSTNAME".service
    sudo systemctl disable --now avahi-alias@"grafana.$HOSTNAME".service
    sudo systemctl disable --now avahi-alias@"minio.$HOSTNAME".service
fi

echo "# Completed Beholder IoT Docker script."