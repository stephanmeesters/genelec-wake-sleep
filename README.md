# Genelec-CLI
This app provides a way to trigger wake or sleep of all connected Genelec SAM monitors through the GLM network adapter.

Based on reverse engineering work by Mark Bergsma https://github.com/markbergsma/genlc

## Usage

### Install using Cargo

```
cargo install --git https://github.com/stephanmeesters/genelec-wake-sleep
```

### Windows

Wake your devices:
```
genelec-cli.exe wake
```

and sleep:
```
genelec-cli.exe sleep
```

### Linux (RPM and DEB packages)

Install the RPM or DEB package. Then you can use:

```
genelec-cli wake
genelec-cli sleep
```

Optionally install the systemd service for auto wake and sleep on boot and shutdown:
```
sudo systemctl enable genelec-wake.service
sudo systemctl enable genelec-sleep.service
```

### Mac
To-do (see csharp-v1 branch)
