# Beholder RPA IoT

A multi-faceted, micrservices-based approach to robotic process automation.

A Beholder agent runs as a self-contained IoT Edge device that can either act as a stand-alone node or be orchestrated as part of a brood of Beholder agents.

<img width="2487" alt="Screen Shot 2021-08-22 at 11 05 18 PM" src="https://user-images.githubusercontent.com/760674/130384500-17de5e03-7790-43d5-991d-c08ef1f673d8.png">

## Comprised of the following skeuomorphic parts:

- Cortex - Browser-based admin/orchestration interface (NextJS) [![Cortex CD](https://github.com/beholder-rpa/beholder-iot/actions/workflows/beholder-cortex-cd.yml/badge.svg)](https://github.com/beholder-rpa/beholder-iot/actions/workflows/beholder-cortex-cd.yml)
- Cerebrum - Node-RED with custom nodes [![Cerebrum CD](https://github.com/beholder-rpa/beholder-iot/actions/workflows/beholder-cerebrum-cd.yml/badge.svg)](https://github.com/beholder-rpa/beholder-iot/actions/workflows/beholder-cerebrum-cd.yml)
- Occipital - Routines for Object Detection using OpenCV [![Occipital v1 CD](https://github.com/beholder-rpa/beholder-iot/actions/workflows/beholder-occipital-v1.yml/badge.svg)](https://github.com/beholder-rpa/beholder-iot/actions/workflows/beholder-occipital-v1.yml)
- Stalk - USB Gadget that mimics a Keyboard/Mouse/Joystick to send input to a desktop (OTG overlay + dotnet core) [![Stalk v2 CD](https://github.com/beholder-rpa/beholder-iot/actions/workflows/beholder-stalk-v2-cd.yml/badge.svg)](https://github.com/beholder-rpa/beholder-iot/actions/workflows/beholder-stalk-v2-cd.yml)
- Epidermis - Allows for IO to/from the beholder [![Epidermis v1 CD](https://github.com/beholder-rpa/beholder-iot/actions/workflows/beholder-epidermis-v1.yml/badge.svg)](https://github.com/beholder-rpa/beholder-iot/actions/workflows/beholder-epidermis-v1.yml)
## Backbone

- PostgreSQL - DB
- Traefik - L7 Proxy & Routing
- Dapr - service invocation, state, and pub/sub blocks.
- Redis - Cache/State Store
- EMQx - MQTT based Message Broker
- Grafna - Analytics
- Jaeger - Tracing

## Connectors:

Beholder Addon - WoW Addon that provides visual data for the Beholder to see and act upon.

# Getting Started

 ### Hardware:
 - Raspberry Pi 4 (Rev 1.2 4GB+ model recommended)

In order to act as a USB device, you'll need a USB C to A Cable

### Initial Raspberry Pi 4 Setup:

Once you've got the hardware in place, you'll need to create a base SD image that contains the Beholder software that
will run on your IoT device. For the Raspberry Pi 4, this image creation process has been automated. See [this](https://github.com/beholder-rpa/beholder-iot-image-builder-rpi4) repository
for instructions on how to create an image.

### Updating

Beholder will auto-update when new containers become available. Additionally, it will pull git changes on startup.

To manually update, ```ssh``` into the device. cd into /beholder. Run ```sudo git pull```. Then, ```sudo reboot now```.


# Developing Beholder
 
### Software:
 - PowerShell
 - Git
 - Docker
 - dotNet Core 5
 - VSCode (or your favorite editor)
 - VSCode Remote extension

It is recommended that front-end (Beholder Cortex) development occur on a local machine, or, via remotely connecting to a running Raspberry Pi instance via the Visual Studio Code Remote Remote - SSH plugin.

> Note: If you've ssh'd into the Beholder before, use ```ssh-keygen -R beholder-01.local``` to clear the previous host key.

By default the environment that runs from the image is a production build, to utilize a dev build, ssh to the Beholder IoT device, cd into the beholder directory and run yarn prod-down, then run yarn up. Now, utilize the VSCode Remote SSH extension to connect to the Beholder IoT device and make changes to the Beholder Cortex files as desired.
