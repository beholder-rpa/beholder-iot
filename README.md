# Beholder RPA

A multi-faceted approach to robotic process automation.

A Beholder agent runs as a self-contained IoT Edge device that can either act as a stand-alone node or be orchestrated as part of a brood of Beholder agents.

## Comprised of the following skeuomorphic parts:

- Eye - High-Speed Screen Capture and image processor (Windows Desktop/dotnet core)
- Stalk - USB Gadget that mimics a Keyboard/Mouse/Joystick to send input to a desktop (OTG overlay + dotnet core)
- Cortex - Browser-based admin/orchestration interface (NextJS)
- Telekinesis - Obtain and interact with processes on the OS (Windows Desktop/.Net Core)

## Foundational

- Cerebrum - Functions-as-a-Service that allows runtime business logic (EspressoV8)
- Medial - Event Sourcing (Generic event subscriber)
- Nexus - Message Broker (RabbitMQ MQTT)

## Backbone

- PostgreSQL - DB
- Traefik - L7 Proxy & Routing
- Dapr - service invocation, state, and pub/sub blocks.
- Redis - Cache/State Store
- Grafna - Analytics

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

```ssh`` into the device. cd into /beholder. Run ```git pull```. Then, ```sudo reboot now```.

> TODO: Beholder should pull pre-built images in the non-developer mode to reduce initial startup time. As part of this, watchtower can auto-update (using a :prod tag or something similar)

# Developing Beholder
 
### Software:
 - PowerShell
 - Git
 - Docker
 - dotNet Core 5
 - VSCode (or your favorite editor)
 - VSCode Remote extension

It is recommended that front-end (Beholder Cortex) development occur via remotely connecting to a running Raspberry Pi instance via the Visual Studio Code Remote Remote - SSH plugin.

By default the environment that runs from the image is a production build, to utilize a dev build, ssh to the Beholder IoT device, cd into the beholder directory and run yarn prod-down, then run yarn up. Now, utilize the VSCode Remote SSH extension to connect to the Beholder IoT device and make changes to the Beholder Cortex files as desired.