#!/bin/sh
if [[ ! -f /data/package.json ]]; then
  echo "package.json not found in data directory; copying files from seed data"
  cp -r /data-seed/* /data
else
  echo "package.json already exists."
fi

npm --no-update-notifier --no-fund start --cache /data/.npm -- --userDir /data
