# pi-beholder

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
- Nginx - L7 Proxy & Routing
- Grafna - Analytics

## Connectors:

Beholder Addon - WoW Addon that provides visual data for the Beholder to see and act upon.

# Getting Started

The following is required:

### Hardware:
 - Raspberry Pi 4 (Rev 1.2 4GB model recommended)
 - USB C to A Cable

### Initial Raspberry Pi 4 Setup:

See ./beholder-image-builder-rpi4/README.md for instructions on building a SD image pre-configured to run Pi-Beholder

# Developing Beholder
 
### Software:
 - NodeJS
 - Yarn
 - Git
 - Docker
 - dotNet Core 3.1
 - VSCode (or your favorite editor)
   - VSCode Remote extension

