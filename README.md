# ButtPlugin for Koikatsu
Adds basic support for [buttplug.io](https://buttplug.io/) compatible devices in Koikatsu.

This plugin is for **linear** devices. I'm not motivated to add other device types by myself, but PRs are welcome.

The full list of linear devices supported by the Buttplug project at the time of writing (skimmed from [this file](https://github.com/buttplugio/buttplug/blob/32253e0c19cb83f0ce326ee5019e5db7f139651d/buttplug/buttplug-device-config/buttplug-device-config.yml)):
* Fleshlight Launch
* Kiiroo Keon
* Kiiroo Onyx
* Kiiroo Onyx 2
* Kiiroo Onyx 2.1
* Kiiroo Onyx+
* Kiiroo Onyx+ Realm Edition
* Kiiroo Titan 1.1
* RealTouch
* The Handy
* Vorze Piston

(Note: all of these devices are onaholes.)

## Installation
Prerequisites:
* Install [Intiface Desktop](https://intiface.com/desktop/).
* Install [BepInEx 5.3](https://github.com/BepInEx/BepInEx/releases) or later.
* Install [KKAPI 1.10.0](https://github.com/IllusionMods/IllusionModdingAPI) or later.

Download the DLLs from the latest release and move them into BepInEx\plugins under your game directory.

## How to use
1. Open Intiface Desktop.
1. Start the server.
1. Add the device you want to use.
1. Disconnect Intiface from the server, but don't close it (this step is important).
1. Start Koikatsu in either desktop or VR mode.
1. You know the rest.

All Insert animations should be accurately synced to the device. Some Receive animations will be out of sync because I haven't got around to them yet.

## Configuration
In Plugin Settings > ButtPlugin, you can set the following parameters:
* The maximum speed your device is capable of ("Maximum strokes per minute"). Based on this value, ButtPlugin will slow down H scene animations if necessary, to keep the immersion. (You'll still be in control of speed, but it will be relative to how fast your toy can go.) The part right before climax will also be slowed down. Keep in mind that the plugin never goes below 70% stroke length, so choose a number your device can reliably maintain at that setting.
* The latency between your display and your device. Set a negative value if your device is faster than your display. (No way to calibrate, you have to "experiment".)
* The Buttplug server address (usually localhost:12345).
