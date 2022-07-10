# BepInEx LoveMachine ([日本語](Documentation/Japanese))
![.NET](https://github.com/Sauceke/LoveMachine/actions/workflows/commit.yml/badge.svg)
[![Download](https://img.shields.io/github/downloads/Sauceke/LoveMachine/total)][installer]

| [⬇ Download][installer] |
|----|

Adds support for [some computer-controlled sex toys](#supported-devices) in the following games:



## Installation
Download and run the [installer]. If you encounter the "Windows protected your PC" message, click More info > Run anyway.

## How to use
1. Open Intiface Desktop.
1. Click Server Status > Start Server.
1. Turn on the device you want to use. You might have to pair it as well.
1. Start the game in either desktop or VR mode.
1. Start an H scene and enjoy 😏

The Space key acts as a kill switch for all devices while in-game. To reactivate your devices, press F8. Both of these key bindings can be modified under Plugin Settings > LoveMachine > Kill Switch Settings.

⚠ In certain games, the kill switch may not work if the BepInEx console is open while playing in VR, because it can steal focus from the game window. It is recommended to disable the console.

## How it works, limitations
* LoveMachine analyzes the movement of certain bones in female characters (hands, crotch, breasts, mouth) at the start of each animation loop, to determine the exact timing of the up-strokes.
* The stroking movement (and the intensity oscillation for vibrators) will be matched to the movements of the bone closest to the male character's balls as recorded during calibration (this messes up syncing with ball licking animations, but works for just about everything else).
* As the whole thing is based on bone positions, this will only work for reasonably sized and proportioned characters. Abusing SliderUnlocker is not recommended.

## Depth control
We have experimental support for two depth sensing toys:
* [Lovense Calor]
* The [Hotdog], a DIY device for transparent sleeves

The Calor works in Koikatsu and Koikatsu Sunshine, while the Hotdog only works in Koikatsu Sunshine. Both were only tested with penetrative scenes. Depth control support in other games will come later.

Both devices are **disabled by default**, you first have to enable the one you are using in [Plugin Settings > LoveMachine > Experimental Features](#experimental-features), then **restart the game**.

Whichever of the two you use, you need to turn it on **before** launching the game. When using a Hotdog, you must start the [Hotdog Server] before the game as well.

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
* **Rotators:** indicates that the device has rotator functionality AND that this functionality is supported by Intiface.
* **Threesome Role:** Which girl the device is assigned to in a threesome. This also affects other scenes - if a device is assigned to second girl, it will not be activated in Standard scenes.
* **Body Part:** Selects the body part that will be tracked by the device. Defaults to Auto (which means it will find the one closest to the player's balls). Can be used to re-enact TJ/FJ with alternating movement using two devices. In Koikatsu and KKS, it also tracks fondling/fingering movements.
* **Test Slow:** Tests the device with 3 slow strokes
* **Test Fast:** Tests the device with 3 fast strokes
* **Save device assignments:** If enabled, the Threesome Role and Body Part attributes will be saved for all devices. Disabled by default.

### Experimental Features
Since these features may mess with some of your peripherals, they are turned off by default. After turning them on, you also have to restart the game for the changes to take effect. It is best if you close Intiface while using these features.
* **Enable Lovense Calor depth control:** Enables control of H scenes using depth data from a [Lovense Calor].
* **Enable Hotdog depth control (KKS only):** Enables control of H scenes using depth data from a [Hotdog].

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
* **Slow Stroke Zone:** The range of the stroking motion when going slow. 0% is the bottom, 100% is the top.
* **Fast Stroke Zone:** The range of the stroking motion when going fast. 0% is the bottom, 100% is the top.

If you get bored of the "standard" features of this plugin, try experimenting a bit with the following settings:
* **Stroke Length Realism:** How much the stroke length should match the animation. 0% means every stroke will use the full available length. 100% means every stroke will be scaled to its in-game length.
* **Hard Sex Intensity:** How fast your stroker will fall during hard sex animations. 100% is twice as fast as 0% and feels much rougher (at least on a Handy). I'm not responsible for any injuries that may occur due to the use of LoveMachine.
* **Orgasm Depth:** The position of the stroker during orgasm.
* **Orgasm Shaking Frequency:** How many strokes to do per second during orgasm.

### Vibration Settings
* **Update Frequency (per second):** How often to send commands to vibrators. Too often might DoS your vibrator, too scarcely will feel erratic. Defaults to 10.
* **Vibration With Animation:** If enabled, vibration intensity will oscillate up and down in sync with the action. If disabled, the intensity will depend on how fast you go, but it will otherwise stay the same.
* **Vibration Intensity Range:** If Vibration With Animation is enabled, vibration intensity will oscillate between these two values.

### Rotator Settings
* **Rotation Speed Ratio:** The speed ratio for rotation. 0% is no rotation, 100% is full speed rotation. Default is 50%.
* **Rotation Direction Change Chance:** The direction of rotation changes with the probability of this setting. Default is 30%.

## Contributing
PRs for onboarding new games are welcome. The process is relatively simple and requires barely any coding. See the PlayHome implementation for reference. PRs for supporting new device types are also welcome.

This mod is provided free of charge, but I do accept donations. If you'd like to boost my morale, please check me out on [Patreon].

### Code contributors
* Sauceke
* nhydock
* hogefugamoga
* RPKU

### Sponsors
* [ManlyMarco]
* AkronusWings
* AstralClock
* Benos Hentai
* Bri
* CBN ヴい
* CROM
* EPTG
* funnychicken
* GOU YOSIHIRO
* Greg
* kai harayama
* kuni
* Nemi
* RP君
* Shakes
* Taibe
* Taka Yami
* tanu
* TrashTaste
* tutinoko
* uruurian
* Wel Adunno
* yamada tarou
* ふ
* 猛 羽場
* 终晓
* 郁弥 中村

## Acknowledgements
This mod would not have been possible without the [BepInEx] plugin framework and, of course, the [Buttplug.io] project.

<!-- own links -->
[installer]: https://github.com/Sauceke/LoveMachine/releases/latest/download/LoveMachineInstaller.exe
[Hotdog]: https://sauceke.github.io/hotdog
[Hotdog Server]: https://github.com/Sauceke/hotdog/releases/latest/download/HotdogServer.exe
[Patreon]: https://www.patreon.com/sauceke



<!-- other links -->
[ManlyMarco]: https://github.com/ManlyMarco
[BepInEx]: https://github.com/BepInEx
[AGHVR]: https://github.com/Eusth/AGHVR
[IOVR]: https://github.com/Eusth/IOVR
