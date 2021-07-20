# ButtPlugin for Koikatsu
Adds support for [buttplug.io](https://buttplug.io/) compatible devices in Koikatsu.

The list of devices supported by the Buttplug project is maintained [here](https://iostindex.com/?filter0ButtplugSupport=4). This plugin is for **linear** (moving back-and-forth) and **vibrating** devices; other types of sex toys will not work with this plugin, even if they are on the list. PRs for extending coverage are welcome.

Currently all linear devices covered by Buttplug are onaholes.

## Installation
Prerequisites:
* Install [Intiface Desktop](https://intiface.com/desktop/).
* Install [BepInEx 5.3](https://github.com/BepInEx/BepInEx/releases) or later.

**If you currently have v1.1.0 or v1.1.0 installed:** Delete ``Koikatsu Party\BepInEx\plugins\KK_ButtPlugin.dll`` before proceeding.

Download **ButtPlugin.zip** from the latest release and drag the folder in the zip file into your Koikatsu installation folder. If you've done it right, there should be a folder called ``Koikatsu Party\BepInEx\plugins\KK_ButtPlugin`` with some DLL files inside.

## How to use
1. Open Intiface Desktop.
1. Start the server.
1. Add the device you want to use. You might have to pair it first.
1. Disconnect Intiface from the server, but don't close it (this step is important).
1. Start Koikatsu in either desktop or VR mode.
1. You know the rest.

An effort was made to sync every animation in vanilla Koikatsu to device actions with an acceptable accuracy, at least where it makes sense. If an animation feels out of sync, and it's not a latency problem, please:
1. Tell us about it by opening an issue. State the exact pose, pose modifier and excitement state.
1. If you're a developer, try tweaking the animations.json file. Don't forget to give us a PR.

## Configuration
<img src="https://user-images.githubusercontent.com/76826783/126218961-e75500a1-bff4-4ac5-aa52-80f435461a8b.jpg">
In Plugin Settings > ButtPlugin, you can set the following parameters:

### Device List
* **Connect:** Reconnect to the Buttplug server.
* **Scan:** Scan for devices.
* **Stroker:** indicates that the device has back-and-forth movement functionality AND that this functionality is supported by Buttplug.
* **Vibrators:** indicates that the device has vibrator functionality AND that this functionality is supported by Buttplug.
* **Threesome role:** Which girl the device is assigned to in a threesome. This also affects other scenes - if a device is assigned to second girl, it will not be activated in Standard scenes.

### Network
* **WebSocket address:** The Buttplug server address (usually localhost:12345).

### Stroker settings
* **Latency (ms):** The latency between your display and your device. Set a negative value if your device is faster than your display. (No way to calibrate, you have to "experiment".)
* **Maximum strokes per minute:** The maximum speed your stroker is capable of (or your slowest stroker if you have more than one). Based on this value, ButtPlugin will slow down H scene animations if necessary, to keep the immersion. (You'll still be in control of speed, but it will be relative to how fast your toy can go.) The part right before climax will also be slowed down. Keep in mind that the plugin never goes below 70% stroke length, so choose a number your device can reliably maintain at that setting.

### Vibration settings
* **Enable Vibrators:**
  * Male: vibrators will only react to what the male character feels
  * Female: vibrators will only react to what the female character feels
  * Both: vibrators will react to everything
  * Off: vibrators will not engage
* **Update Frequency (per second):** How often to send commands to vibrators. Too often might DoS your vibrator, too scarcely will feel erratic.
* **Vibration With Animation:** If enabled, vibration intensity will oscillate up and down in sync with the action. If disabled, the intensity will depend on how fast you go, but it will otherwise stay the same.


