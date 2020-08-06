#!/bin/bash

echo "# Executing Beholder IoT OTG script..."

cd /sys/kernel/config/usb_gadget/
mkdir -p beholder && cd beholder

echo 0x1d6b > idVendor # 0x1d6b = Linux Foundation 0x45e = Microsoft
echo 0x0104 > idProduct # Multifunction Composite Gadget
echo 0x0100 > bcdDevice # v1.0.0
echo 0x0200 > bcdUSB # USB2

# https://www.usb.org/defined-class-codes
# Although this post http://irq5.io/2016/12/22/raspberry-pi-zero-as-multiple-usb-gadgets/
# indicates that this needs to be 0xEF, 0x02, 0x01 respectively,
# on Windows 10, the following device class/subclass/protocol seems to work without issue.

echo 0x00 > bDeviceClass # Use class code info from Interface Descriptors
echo 0x00 > bDeviceSubClass
echo 0x00 > bDeviceProtocol
echo 0x08 > bMaxPacketSize0 # Maximum packet size for the device, valid values are 8, 16, 32, 64.

# Add English Locale information strings
mkdir -p strings/0x409
echo "5eaf00d0b57ac1e" > strings/0x409/serialnumber
echo "BaristaLabs, LLC" > strings/0x409/manufacturer
echo "Beholder" > strings/0x409/product

# Create HID Functions
# See https://www.usb.org/hid

# Keyboard - Standard Keyboard
mkdir -p functions/hid.usb0
echo 1 > functions/hid.usb0/protocol
echo 1 > functions/hid.usb0/subclass
echo 8 > functions/hid.usb0/report_length
printf '05010906a101050719e029e71500250175019508810295017508810395057501050819012905910295017503910395067508150025ff0507190029658100c0' | xxd -r -ps > functions/hid.usb0/report_desc

# Mouse - 5-button with scroll+tilt high resolution pointer.
mkdir -p functions/hid.usb1
echo 2 > functions/hid.usb1/protocol
echo 1 > functions/hid.usb1/subclass
echo 7 > functions/hid.usb1/report_length
printf '05010902a1010902a1020901a1000509190129051500250175019505810275039501810305010930093116018026ff7f751095028106a1020948150025013501450475029501a4b10209381581257f3500450075088106c0a1020948b4b102350045007504b103050c0a38021581257f75088106c0c0c0c0' | xxd -r -ps > functions/hid.usb1/report_desc

# Joystick - 2-axis, throttle, 4 button, 4-way hat.
mkdir -p functions/hid.usb2
echo 0 > functions/hid.usb2/protocol
echo 0 > functions/hid.usb2/subclass
echo 4 > functions/hid.usb2/report_length
printf '050115000904a101050209bb1581257f75089501810205010901a1000930093195028102c00939150025033500460e0165147504950181020509190129041500250175019504550065008102c0' | xxd -r -ps > functions/hid.usb2/report_desc

# ECM Network
mkdir -p functions/ecm.usb0
HOST="00:dc:c8:f7:75:14" # "HostPC"
SELF="00:dd:dc:eb:6d:a1" # "BadUSB"
echo $HOST > functions/ecm.usb0/host_addr
echo $SELF > functions/ecm.usb0/dev_addr

# Mass Storage Device
# mkdir -p functions/mass_storage.usb0
# echo 0 > functions/mass_storage.usb0/stall
# echo 0 > functions/mass_storage.usb0/lun.0/cdrom
# echo 1 > functions/mass_storage.usb0/lun.0/ro
# echo 0 > functions/mass_storage.usb0/lun.0/nofua
# echo /opt/disk.img > functions/mass_storage.usb0/lun.0/file

# End functions

# Create configuration
mkdir -p configs/c.1
mkdir -p configs/c.1/strings/0x409

echo 0x80 > configs/c.1/bmAttributes # Only bus powered
echo 250 > configs/c.1/MaxPower # 250ma
echo "Beholder" > configs/c.1/strings/0x409/configuration

# Link HID functions to configuration
ln -s functions/hid.usb0 configs/c.1/
ln -s functions/hid.usb1 configs/c.1/
ln -s functions/hid.usb2 configs/c.1/

ln -s functions/ecm.usb0 configs/c.1/
# ln -s functions/mass_storage.usb0 configs/c.1/

# OS descriptors (Make it work on windows)
# Although this post http://irq5.io/2016/12/22/raspberry-pi-zero-as-multiple-usb-gadgets/
# indicates that the os_desc must be defined,
# on Windows 10, omitting the os_desc seems to work without issue.

# mkdir -p os_desc
# echo 1       > os_desc/use
# echo 0xcd    > os_desc/b_vendor_code
# echo MSFT100 > os_desc/qw_sign

# ln -s configs/c.1 os_desc

# end OS Descriptors

# Wait for udevd to process device creation events.
udevadm settle -t 5 || :

# Enable gadget
ls /sys/class/udc > UDC
ifup usb0
service dnsmasq restart

echo "# Completed Beholder IoT OTG script."