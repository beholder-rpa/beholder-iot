#!/bin/bash

echo "# Executing Beholder IoT Docker script..."

[ "$1" = up ] &&
    yarn up

[ "$1" = down ] &&
    yarn down

echo "# Completed Beholder IoT Docker script."