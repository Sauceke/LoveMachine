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
* Install [Initface Desktop](https://intiface.com/desktop/).
* Install [BepInEx 5.3](https://github.com/BepInEx/BepInEx/releases) or later.
Download the DLLs from the latest release and move them into BepInEx\plugins under your game directory.

## How to use
1. Open Initface Desktop.
1. Start the server.
1. Add the device you want to use.
1. Disconnect Intiface from the server.
1. Start Koikatsu in either desktop or VR mode.
1. You know the rest.

The plugin handles most of the BJ, HJ, TJ, cowgirl and chair cowgirl animations well enough. For some of the animations (doggy, missionary etc.), the movement gets reversed (going down when it should be going up). Still not sure how to fix this.

## Configuration
In Plugin Settings > ButtPlugin, you can set the maximum speed your device is capable of ("Maximum strokes per minute"). Based on this value, ButtPlugin will slow down H scene animations if necessary, to keep the immersion. (You'll still be in control of speed, but it will be relative to how fast your toy can go.) The part right before climax will also be slowed down. Keep in mind that the plugin never goes below 70% stroke length, so choose a number your device can reliably maintain at that setting.

You can also change the Buttplug server address in this menu (usually localhost:12345).
