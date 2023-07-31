# Genelec Wake Sleep
The Intelligent Signal Sensing (ISS) feature of Genelec monitors does not always work well because the audio source sends a digital clock signal that prevents sleep. This app can be used to trigger wake or sleep of all connected Genelec devices.

Based on reverse engineering work from https://github.com/markbergsma/genlc/tree/master

## Usage
To wake or sleep, run wake-genelec.bat or sleep-genelec.bat, respectively.

Or run the command directly, for wake:
```
GenelecApp.exe wake
```

and sleep:
```
GenelecApp.exe sleep
```