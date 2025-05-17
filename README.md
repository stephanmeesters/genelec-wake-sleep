# Genelec Wake Sleep
The Intelligent Signal Sensing (ISS) feature of Genelec monitors does not always work well because the audio source sends a digital clock signal that prevents sleep. This app can be used to trigger wake or sleep of all connected Genelec SAM monitors through the GLM network adapter.

Based on reverse engineering work by Mark Bergsma https://github.com/markbergsma/genlc

## Usage

### Windows
To wake or sleep, run wake-genelec.bat or sleep-genelec.bat, respectively.

Or run the command directly, for wake:
```
GenelecApp.exe wake
```

and sleep:
```
GenelecApp.exe sleep
```

### Linux (Debian)
A recent version of HIDAPI is required (tested using 0.14.0). Use the included .so file or compile the library from https://github.com/libusb/hidapi

Run the command for wake:
```
sudo ./GenelecApp wake
```

and sleep:
```
sudo ./GenelecApp sleep
```

To be able to access the USB device without sudo, install the included HID rule:
```
cp 70-genelecapp-hid.rules /etc/udev/rules.d/
sudo udevadm control --reload-rules && sudo udevadm trigger
```

### Mac (Intel/Apple Silicon) -- untested
Install HIDAPI using [Brew](https://brew.sh/):
```
brew install hidapi
```

Run the command for wake:
```
sudo ./GenelecApp wake
```

and sleep:
```
sudo ./GenelecApp sleep
```

To run without sudo, try adding GenelecApp to "Enable access for assistive devices" in "System Preferences > Security & Privacy > Privacy > Accessibility".
