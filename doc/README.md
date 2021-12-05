# BepInEx LoveMachine ([日本語](マニュアル.md))
[![.NET](https://github.com/Sauceke/BepInEx.LoveMachine/actions/workflows/commit.yml/badge.svg)](https://github.com/Sauceke/BepInEx.LoveMachine/actions/workflows/commit.yml)

Adds support for [buttplug.io](https://buttplug.io/) compatible strokers and vibrators in the following games:
| Game                           | Developer        | VR supported |
|--------------------------------|------------------|--------------|
| Koikatsu                       | Illusion         | Yes          |
| Koikatsu Party                 | Illusion         | Yes          |
| Koikatsu Sunshine              | Illusion         | Yes          |
| Honey Select 2                 | Illusion         | Yes          |
| PlayHome                       | Illusion         | Yes          |
| After Service Gangbang Addicts | MICONISOMI       | Yes, wtih [AGHVR](https://github.com/Eusth/AGHVR) |
| Our Apartment                  | Momoiro Software | No           |

Buttplug.io supports [over 180 devices](https://iostindex.com/?filter0ButtplugSupport=4). Some of the devices that were actually tested with the mod:
* [The Handy](https://www.thehandy.com/?ref=saucekebenfield&utm_source=saucekebenfield&utm_medium=affiliate&utm_campaign=The+Handy+Affiliate+program)
* KIIROO KEON
* Lovense Max 2
* Lovense Diamo
* Lovense Domi 2
* The Xbox gamepad

This plugin is for **linear** (moving back-and-forth) and **vibrating** devices; other types of sex toys will not work with this plugin, even if they are supported by buttplug.io.

## Installation
Prerequisites:
* Install [Intiface Desktop](https://intiface.com/desktop/).
* Install [BepInEx 5.3](https://github.com/BepInEx/BepInEx/releases) or later.

Go to the [latest release page](https://github.com/Sauceke/BepInEx.LoveMachine/releases), download the ZIP that corresponds to the game you want to patch, and extract it into your game's installation folder. If you've done it right, there should be a folder in your game's directory called ``BepInEx\plugins\KK_LoveMachine`` or something similar.

**For After Service Gangbang Addicts,** please install the **32 bit** version of [BepInEx](https://github.com/BepInEx/BepInEx/releases). Download the ZIP whose name starts with ``BepInEx_x86...`` and extract it into your game folder. Then extract ``LoveMachine_for_Houkago_Rinkan_Chuudoku.zip`` into your game folder.

## How to use
1. Open Intiface Desktop.
1. Click Server Status > Start Server.
1. Turn on the device you want to use. You might have to pair it as well.
1. Start the game in either desktop or VR mode.
1. Start an H scene and enjoy 😏

The Space key acts as a kill switch for all devices while in-game. To reactivate your devices, press F8. Both of these key bindings can be modified under Plugin Settings > LoveMachine > Kill Switch Settings.

## How it works, limitations
* LoveMachine analyzes the movement of certain bones in female characters (hands, crotch, breasts, mouth) at the start of each animation loop, to determine the exact timing of the up-strokes.
* The stroking movement (and the intensity oscillation for vibrators) will be matched to the movements of the bone closest to the male character's balls as recorded during calibration (this messes up syncing with ball licking animations, but works for just about everything else).
* As the whole thing is based on bone positions, this will only work for reasonably sized and proportioned characters. Abusing SliderUnlocker is not recommended.

## Configuration
In Plugin Settings > LoveMachine, you can set the following parameters:

### Animation Settings (Koikatsu and KKS only)
* **Reduce animation speeds:** If enabled, LoveMachine will try to ensure animations will never get faster than what your device is capable of (see Maximum strokes per minute). Turned on by default. May interfere with other mods.
* **Simplify animations:** If enabled, LoveMachine will remove motion blending from animations. Motion blending messes up the timing algorithm, so this setting is essential if you want real immersion, especially with Sideloader animations. Turned on by default. May interfere with other mods, though unlikely.

### Device List
This is where all your devices connected to Intiface are listed.
* **Connect:** Reconnect to the Intiface server.
* **Scan:** Scan for devices.
* **Stroker:** indicates that the device has back-and-forth movement functionality AND that this functionality is supported by Intiface.
* **Vibrators:** indicates that the device has vibrator functionality AND that this functionality is supported by Intiface.
* **Threesome Role:** Which girl the device is assigned to in a threesome. This also affects other scenes - if a device is assigned to second girl, it will not be activated in Standard scenes.
* **Body Part:** Selects the body part that will be tracked by the device. Defaults to Auto (which means it will find the one closest to the player's balls). Can be used to re-enact TJ/FJ with alternating movement using two devices. In Koikatsu and KKS, it also tracks fondling/fingering movements.
* **Test Slow:** Tests the device with 3 slow strokes
* **Test Fast:** Tests the device with 3 fast strokes
* **Save device assignments:** If enabled, the Threesome Role and Body Part attributes will be saved for all devices. Disabled by default.

### Kill Switch Settings
Safety measure to avoid hurting yourself if the sex gets too rough or something goes wrong. By default, pressing Spacebar will immediately stop all connected devices.
* **Emergency Stop Key Binding:** Sets the keystroke for activating the kill switch (Space by default).
* **Resume Key Binding:** Sets the keystroke for deactivating the kill switch (F8 by default).

### Network Settings
* **WebSocket address:** The Buttplug server address (usually localhost:12345).

### Stroker Settings
* **Latency (ms):** The latency between your display and your device. Set a negative value if your device is faster than your display. (No way to calibrate, you have to "experiment".)
* **Maximum strokes per minute:** The maximum speed your stroker is capable of (or your slowest stroker if you have more than one). Based on this value, LoveMachine will slow down H scene animations if necessary, to keep the immersion. (You'll still be in control of speed, but it will be relative to how fast your toy can go.) The part right before climax will also be slowed down.

You can define two "stroke zones", one for fast movement and one for slow movement. These zones gradually change into one another as the speed increases/decreases.
* **Slow Stroke Zone:** The range of the stroking motion when going slow. 0% is the top, 100% is the bottom.
* **Fast Stroke Zone:** The range of the stroking motion when going fast. 0% is the top, 100% is the bottom.

If you get bored of the "standard" features of this plugin, try experimenting a bit with the following setting:
* **Hard Sex Intensity:** How fast your stroker will fall during hard sex animations. 100% is twice as fast as 0% and feels much rougher (at least on a Handy). I'm not responsible for any injuries that may occur due to the use of LoveMachine.

### Vibration Settings
* **Update Frequency (per second):** How often to send commands to vibrators. Too often might DoS your vibrator, too scarcely will feel erratic. Defaults to 10.
* **Vibration With Animation:** If enabled, vibration intensity will oscillate up and down in sync with the action. If disabled, the intensity will depend on how fast you go, but it will otherwise stay the same.
* **Vibration Intensity Range:** If Vibration With Animation is enabled, vibration intensity will oscillate between these two values.

## Contributing
PRs for onboarding new games are welcome. The process is relatively simple and requires barely any coding. See the PlayHome implementation for reference. PRs for supporting new device types are also welcome.

This mod is provided free of charge, but I do accept donations. If you'd like to boost my morale, please check me out on [Patreon](https://www.patreon.com/sauceke).

### Code contributors
* nhydock
* Sauceke

### Sponsors
* [ManlyMarco](https://github.com/ManlyMarco)
* AkronusWings
* Benos Hentai
* EPTG
* Shakes
* Taibe
* Taka Yami
* tanu
* uruurian
* Wel Adunno
* yamada tarou

## Acknowledgements
This mod would not have been possible without the [BepInEx](https://github.com/BepInEx) plugin framework and, of course, the [Buttplug](https://buttplug.io/) project.
