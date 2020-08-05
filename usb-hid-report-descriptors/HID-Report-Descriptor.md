# How to dump report descriptor

## On Windows

- [USB Device Tree Viewer](https://www.uwe-sieber.de/usbtreeview_e.html#download) (Recomended 2019-09-26)

- [USBlyzer](http://www.usblyzer.com/download.htm) (Fully functional 33-day trial)

## lsusb on Linux

http://www.slashdev.ca/2010/05/08/get-usb-report-descriptor-with-linux/

Find vendor id of device interested(046d)
  
 $ lsusb
    ...
    Bus 002 Device 007: ID 046d:c01d Logitech, Inc. MX510 Optical Mouse
    ...
    
Find device path from dmesg output(2-2.4:1.0)
    
    $ dmesg | grep -i 046d | grep devices | tail -5
Aug 12 09:15:55 desk kernel: [166377.779960] input: Logitech USB-PS/2 Optical Mouse as /devices/pci0000:00/0000:00:04.1/usb2/2-2/2-2.4/2-2.4:1.0/0003:046D:C01D.0067/input/input106
  
Unbind the device and it stops working
  
 \$ sudo -i # echo -n 2-2.4:1.0 > /sys/bus/usb/drivers/usbhid/unbind

Dump report descriptor with lsusb
  
 # lsusb -d046d: -v
...
iInterface 0
HID Device Descriptor:
bLength 9
bDescriptorType 33
bcdHID 1.10
bCountryCode 0 Not supported
bNumDescriptors 1
bDescriptorType 34 Report
wDescriptorLength 77
Report Descriptor: (length is 77)
Item(Global): Usage Page, data= [ 0x01 ] 1
Generic Desktop Controls
Item(Local ): Usage, data= [ 0x02 ] 2
Mouse
Item(Main ): Collection, data= [ 0x01 ] 1
Application
Item(Local ): Usage, data= [ 0x01 ] 1
Pointer
Item(Main ): Collection, data= [ 0x00 ] 0
Physical
Item(Global): Usage Page, data= [ 0x09 ] 9
Buttons
...
  
Bind the deivce and it starts working again
  
 # echo -n 2-2.4:1.0 > /sys/bus/usb/drivers/usbhid/bind # exit

## usbhid-dump on Linux

https://github.com/DIGImend/usbhid-dump
  
To install on debian system
  
 $ sudo apt-get install usbutils
    $ sudo apt-get install hidrd

And dump descriptor and parse it

    $ sudo usbhid-dump -d046d -i255
    001:005:000:DESCRIPTOR         1468982571.405056
     05 01 09 02 A1 01 09 01 A1 00 05 09 19 01 29 08
     15 00 25 01 95 08 75 01 81 02 95 00 81 03 06 00
     FF 09 40 95 02 75 08 15 81 25 7F 81 02 05 01 09
     38 15 81 25 7F 75 08 95 01 81 06 09 30 09 31 16
     01 F8 26 FF 07 75 0C 95 02 81 06 C0 C0

    $ sudo usbhid-dump -d046d -i255 |grep -v : | xxd -r -p | hidrd-convert -o spec
    Usage Page (Desktop),               ; Generic desktop controls (01h)
    Usage (Mouse),                      ; Mouse (02h, application collection)
    Collection (Application),
        Usage (Pointer),                ; Pointer (01h, physical collection)
        Collection (Physical),
            Usage Page (Button),        ; Button (09h)
            Usage Minimum (01h),
            Usage Maximum (08h),
            Logical Minimum (0),
            Logical Maximum (1),
            Report Count (8),
            Report Size (1),
            Input (Variable),
            Report Count (0),
            Input (Constant, Variable),
            Usage Page (FF00h),         ; FF00h, vendor-defined
            Usage (40h),
            Report Count (2),
    ...

# USB NKRO Descriptor

https://github.com/tmk/tmk_keyboard/issues/191

## Soarer's NKRO/6KRO trick

[[Soarer's Converter Descriptor]]

I watched HID reports from soarer's with `usbhid-dump` on Linux(or in report mode) the converter always sends two reports from interface 0(boot) and 2(NKRO) at same time. Interface0(boot keyboard) sends reports in boot protocol format(6KRO). Because report descriptor of interface0 declares Input data as **"Constant"**, in the result OS ignores reports from this interface and uses only NKRO report.

```
$ sudo usbhid-dump  -d16c0:047d -i0 -es -t10000
$ sudo usbhid-dump  -d16c0:047d -i2 -es -t10000
```

Decent host can read the report descriptor correctly, ignores Boot report and uses only NKRO report, while crappy host(or BIOS) cannot read the report descriptor, ignores NKRO report and uses only Boot report.

I didn't confirm but I guess the converter stops interface2(NKRO) when BIOS/UEFI requests boot mode.

It doesn't seem to have switching method between NKRO and Boot keyboard.

Also refer this: http://deskthority.net/workshop-f7/soarer-desparately-needed-t9322-30.html#p210886

> In theory, the keyboard should come up in NKRO with an NKRO descriptor and advertise boot mode capability on ONE endpoint, the BIOS should request boot mode, and then the keyboard should switch to boot mode. However, some BIOSes assume that a keyboard that advertises boot mode actually is running in boot mode, and fail to request it (which is a safe assumption on 99.9% of keyboards, but violates the spec). If a BIOS follows the spec and requests that the boot mode endpoint switch to boot mode, I believe Soarer is shutting down the NKRO endpoint.
>
> Then, per the spec, once the OS comes up and resets all USB devices, it'll parse all the descriptors. His boot mode endpoint intentionally ~~doesn't have a descriptor~~ (has **Constant** descriptor), because there is no way to reliably sense that there's a real OS here, but his NKRO endpoint has one, so the OS ignores everything coming from the boot mode endpoint (but it still has to be sent in case a BIOS sees boot mode capability and assumes it's good to go).

Quote from v1.11 changes:

> Prevented debug and rawhid output when keyboard_protocol is not set (i.e. in BIOS mode).

## Realforce RGB

[[Realforce RGB Descriptor]]

```
Interface0
    Boot keyboard + 6KRO report descriptor
        Modifiers 1byte + constant 1byte + keys 6 bytes = 8
    Endpoint 1IN
    PacketSize 8
    Interval    1

Interface1
    Multimeida
        ReportID=1 + consumer 1byte =2
        ReportID=2 + constant 1byte + NKRO keyboard 232bits(0-231[Right GUI]) = 31
        ReportID=3 + consumer 1byte =2
    Endpoint 2IN
    PakcetSize  32
    Interval    1

Interface2
    Vendor specific
        64byte report
    Endopoint 3IN/4OUT
    PakcetSize  64
    Interval    1
```

Use Boot keyboard interface until its report is full and NKRO keyboard interface is used only when Boot report is not enough. Modifiers is always registered with Boot report. No switching function between Boot and NKRO keyboard.

Modifiers don't affect on keys of NKRO keyboard interface with some OS such as MacOS?

## Logitech MX510 mouse

```
$ sudo usbhid-dump -d 046d -i255 | grep -v : | xxd -r -p | hidrd-convert -o spec
Usage Page (Desktop),               ; Generic desktop controls (01h)
Usage (Mouse),                      ; Mouse (02h, application collection)
Collection (Application),
    Usage (Pointer),                ; Pointer (01h, physical collection)
    Collection (Physical),
        Usage Page (Button),        ; Button (09h)
        Usage Minimum (01h),
        Usage Maximum (08h),
        Logical Minimum (0),
        Logical Maximum (1),
        Report Count (8),
        Report Size (1),
        Input (Variable),
        Report Count (0),
        Input (Constant, Variable),
        Usage Page (FF00h),         ; FF00h, vendor-defined
        Usage (40h),
        Report Count (2),
        Report Size (8),
        Logical Minimum (-127),
        Logical Maximum (127),
        Input (Variable),
        Usage Page (Desktop),       ; Generic desktop controls (01h)
        Usage (Wheel),              ; Wheel (38h, dynamic value)
        Logical Minimum (-127),
        Logical Maximum (127),
        Report Size (8),
        Report Count (1),
        Input (Variable, Relative),
        Usage (X),                  ; X (30h, dynamic value)
        Usage (Y),                  ; Y (31h, dynamic value)
        Logical Minimum (-2047),
        Logical Maximum (2047),
        Report Size (12),
        Report Count (2),
        Input (Variable, Relative),
    End Collection,
End Collection
```

## Apple Magic Keyboard model A1644

https://gist.github.com/tmk/0626b78f73575d5c1efa86470c4cdb18

https://github.com/tmk/tmk_keyboard/issues/606

From:
https://github.com/tmk/tmk_keyboard/wiki/HID-Report-Descriptor
