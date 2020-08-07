#!/bin/bash

echo "# Executing Beholder IoT Docker script..."

[ "$1" = up ] &&
    yarn prod-up

[ "$1" = down ] &&
    yarn prod-down

echo "# Completed Beholder IoT Docker script."