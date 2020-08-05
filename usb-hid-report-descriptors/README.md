USB HID Report Descriptors
---

A collection of various HID Report Descriptors 

To Dump:

Install
```
$ sudo apt-get install usbutils hidrd
```

Use

```
$ sudo usbhid-dump -d1532 -i255 | grep -v : | xxd -r -p | hidrd-convert -o natv > <Descriptor Name, e.g. razer-naga-trinity>
```

View

```
$ hidrd-convert <Descriptor Name, e.g. razer-naga-trinity> -o spec
```

```
$ xxd -p -u <Descriptor Name, e.g. razer-naga-trinity> | tr -d \\n
```