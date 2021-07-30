# BepInEx ButtPlugin ([日本語](マニュアル.md))
Adds support for [buttplug.io](https://buttplug.io/) compatible strokers and vibrators in the following games:
* Koikatsu (VR too)
* Honey Select 2 (VR too)

The full list of devices supported by the Buttplug project is maintained [here](https://iostindex.com/?filter0ButtplugSupport=4). This plugin is for **linear** (moving back-and-forth) and **vibrating** devices; other types of sex toys will not work with this plugin, even if they are on the list. PRs for extending coverage are welcome.

The following devices were used for testing at various points during this project's development:
* The Handy
* KIIROO KEON
* Lovense Diamo
* The Xbox gamepad

## Installation
Prerequisites:
* Install [Intiface Desktop](https://intiface.com/desktop/).
* Install [BepInEx 5.3](https://github.com/BepInEx/BepInEx/releases) or later.

**If you currently have v1.1.0 or v1.1.0 installed:** Delete ``Koikatsu Party\BepInEx\plugins\KK_ButtPlugin.dll`` before proceeding.

Go to the [latest release page](https://github.com/Sauceke/BepInEx.ButtPlugin/releases), download the ZIP that corresponds to the game you want to patch, and drag the BepInEx folder from the zip file into your game's installation folder (it already has a BepInEx folder, but do it anyway). If you've done it right, there should be a folder in your game's directory called ``BepInEx\plugins\KK_ButtPlugin`` with some DLL files inside.

## How to use
1. Open Intiface Desktop.
1. Click Server Status > Start Server.
1. Turn on the device you want to use. You might have to pair it as well.
1. Start the game in either desktop or VR mode.
1. You're welcome.

## How it works, limitations
* ButtPlugin analyzes the movement of certain bones in female characters (hands, crotch, breasts, mouth).
* To do this, we hijack the animator for 10 frames each time a new animation loop is loaded, to do a quick calibration. You will see the animation glitch for a split second when this happens.
* The stroking movement (and the intensity oscillation for vibrators) will be matched to the movements of the bone closest to the male character's balls as recorded during calibration (this messes up syncing with ball licking animations, but works for just about everything else).
* As the whole thing is based on bone positions, this will only work for reasonably sized characters (no lewding 7 inch fairies).
* If you change poses during calibration, it kind of makes a mess of the whole thing and you can only fix it by restarting the game. Make sure you don't interfere with the calibration process.

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
Currently, most of these settings will only work in Koikatsu. HF2 support later.
* **Enable Vibrators:**
  * Male: vibrators will only react to what the male character feels
  * Female: vibrators will only react to what the female character feels
  * Both: vibrators will react to everything
  * Off: vibrators will not engage
* **Update Frequency (per second):** How often to send commands to vibrators. Too often might DoS your vibrator, too scarcely will feel erratic.
* **Vibration With Animation:** If enabled, vibration intensity will oscillate up and down in sync with the action. If disabled, the intensity will depend on how fast you go, but it will otherwise stay the same.


