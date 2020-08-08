#!/bin/bash

dotnet publish -r linux-arm /p:ShowLinkerSizeComparison=true 
pushd ./bin/Debug/netcoreapp3.0/linux-arm/publish
rsync -r ./* pi@beholder-01.local:/home/pi/beholder-stalk
popd