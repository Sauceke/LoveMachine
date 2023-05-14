[日本語](README-jp.md)

# BepInEx LoveMachine
[![.NET][CI Badge]](#)
[![Download][Downloads Badge]][installer]
[![Patreon][Patreon Badge]][Patreon]

[![Download][Download Button]][installer] &nbsp;
[![Watch Demo (NSFW)][Demo Button]][Demo video]

Adds support for [some computer-controlled sex toys](#supported-devices) in the following games:

| Game                        | Developer        | VR support       |
|-----------------------------|------------------|------------------|
| [Custom Order Maid 3D 2]    | Kiss             | ✓                |
| [Holy Knight Ricca]         | Mogurasoft       | ✗                |
| [Honey Select 2]            | Illusion         | ✓                |
| [Houkago Rinkan Chuudoku]   | Miconisomi       | ✓ (with [AGHVR]) |
| [Incubus 2: Camlann]        | Tanpakusitsu     | ✗                |
| [Insult Order]              | Miconisomi       | ✓ (with [IOVR])  |
| [Koikatsu]                  | Illusion         | ✓                |
| [Koikatsu Party]            | Illusion         | ✓                |
| [Koikatsu Sunshine]         | Illusion         | ✓                |
| [Koi-Koi VR: Love Blossoms] | Apricot Heart    | ✓                |
| [Last Evil]                 | Flametorch       | ✗                |
| [Our Apartment]             | Momoiro Software | ✗                |
| [PlayHome]                  | Illusion         | ✓                |
| [RoomGirl]                  | Illusion         | ✗                |
| [Secrossphere]              | Illusion         | ✗                |
| [Succubus Cafe]             | Migi Studio      | ✗                |
| [VR Kanojo]                 | Illusion         | ✓                |

## Supported devices
LoveMachine relies on the [Buttplug.io] project to communicate with toys. At the time of writing, Buttplug.io supports over 200 devices.

This plugin is for **linear** (moving back-and-forth), **vibrating**, **rotating** and **tightening** sex toys.

Some of the devices that were actually tested with the mod:

Strokers
- [The Handy]
- OSR2
- KIIROO KEON

Vibrators
- [Lovense Gush]
- [Lovense Max 2]
- [Lovense Diamo]
- [Lovense Domi 2]
- [Lovense Calor]
- The Xbox gamepad

Rotators
- Vorze A10 Cyclone

The [LoveMachine.Experiments] plugin also adds experimental support for two depth sensing devices ([Lovense Calor] and the [Hotdog]) to Koikatsu and Koikatsu Sunshine.

## Installation
Download and run the [installer]. If you encounter the "Windows protected your PC" message, click More info > Run anyway.

[Intiface Central] must also be installed.

## How to use
1. Open Intiface Central.
1. Click on the big Play button.
1. Turn on the device you want to use.
1. Start the game.

The Space key acts as a kill switch for all devices while in-game. To reactivate your devices, press F8. Both of these key bindings can be modified under Plugin Settings > LoveMachine > Kill Switch Settings.

⚠ In certain games, the kill switch may not work if the BepInEx console is open while playing in VR, because it can steal focus from the game window. It is recommended to disable the console.

## How it works, limitations
- LoveMachine analyzes the movement of certain bones in female characters (hands, crotch, breasts, mouth) at the start of each animation loop, to determine the exact timing of the up-strokes.
- The stroking movement (and the intensity oscillation for vibrators) will be matched to the movements of the bone closest to the male character's balls as recorded during calibration (this messes up syncing with ball licking animations, but works for just about everything else).
- As the whole thing is based on bone positions, this will only work for reasonably sized and proportioned characters.

## Configuration
⚠ IL2CPP games (RoomGirl, Holy Knight Ricca) are not compatible with ConfigurationManager at the moment. Please edit the configuration file instead.

In Plugin Settings > LoveMachine, you can set the following parameters:

### Animation Settings (Koikatsu and KKS only)
- **Simplify animations:** If enabled, LoveMachine will remove motion blending from animations. Motion blending messes up the timing algorithm, so this setting is essential if you want real immersion, especially with Sideloader animations. Turned off by default. May interfere with other mods.

### Device List
This is where all your devices connected to Intiface are listed.
- **Connect:** Connect or reconnect to the Intiface server.
- **Scan:** Scan for devices.

General device settings (all devices):
- **Group Role:** Which girl the device is assigned to in a group scene. This also affects scenes that are not group scenes, e.g. if a device is assigned to second girl, and there is only one girl in the scene, it will not be activated at all.
- **Body Part:** Selects the body part that will be tracked by the device. Defaults to Auto (which means it will find the one closest to the player's balls). Can be used to re-enact TJ/FJ with alternating movement using two devices. In Koikatsu and KKS, it also tracks fondling/fingering movements.
- **Latency (milliseconds):** Latency of sex toys is usually negligible, but if you're experiencing any noticeable delay between your display and your device, use this setting to correct it. There's no way to calibrate this, so you'll have to experiment.
- **Updates per second:** How often to send commands to this device. BLE devices can usually handle about 10-20 commands per second.

Stroker settings:
- **Max Strokes (per minute):** The maximum speed your stroker is capable of at 100% stroke length.
- **Stroke Zone / Slow:** The range of the stroking motion when going slow. 0% is the bottom, 100% is the top.
- **Stroke Zone / Fast:** The range of the stroking motion when going fast. 0% is the bottom, 100% is the top.
- **Smooth Stroking:** Makes the stroking movement less robotic, but not all strokers can handle this. Known to work well on Handy and OSR2 devices. Turned off by default.

Vibrator settings:
- **Intensity Range:** Minimum and maximum vibration intensity allowed for this device. 0% = no vibration, 100% = full strength.
- **Vibration Pattern:** The waveform of the vibration intensity. Available values are Sine, Triangle, Saw, Pulse, Constant, and Custom.
- **Custom Pattern:** Available if Vibration Pattern is set to Custom. You can set the vibration intensity curve using the sliders.

Pressure settings:
- **Pressure Range:** Minimum and maximum pressure allowed on this device, in percentages.
- **Pressure Update Interval (seconds)** How much time it takes for this device to change pressure, in seconds. Defaults to 5.


You may also want to:
- **Save device assignments:** If enabled, the Threesome Role and Body Part attributes will be saved for all devices. Disabled by default.

### Intiface Settings
- **Intiface CLI location:** The path to Intiface CLI. The plugin will attempt to run this program when the game is launched.
- **WebSocket host:** The URL of the host Intiface is running on. Should be `ws://localhost` unless it's running on a remote machine.
- **WebSocket port:** The port Intiface is listening on. Usually `12345`.

### Kill Switch Settings
Safety measure to avoid hurting yourself if the sex gets too rough or something goes wrong. By default, pressing Spacebar will immediately stop all connected devices.
- **Emergency Stop Key Binding:** Sets the keystroke for activating the kill switch (Space by default).
- **Resume Key Binding:** Sets the keystroke for deactivating the kill switch (F8 by default).

### Stroker Settings
- **Stroke Length Realism:** How much the stroke length should match the animation. 0% means every stroke will use the full available length. 100% means every stroke will be scaled to its in-game length.
- **Hard Sex Intensity:** How fast your stroker will fall during hard sex animations. 100% is twice as fast as 0% and feels much rougher (at least on a Handy). I'm not responsible for any injuries that may occur due to the use of LoveMachine.
- **Orgasm Depth:** The position of the stroker during orgasm.
- **Orgasm Shaking Frequency:** How many strokes to do per second during orgasm.

### Rotator Settings
- **Rotation Speed Ratio:** The speed ratio for rotation. 0% is no rotation, 100% is full speed rotation. Default is 50%.
- **Rotation Direction Change Chance:** The direction of rotation changes with the probability of this setting. Default is 30%.

### Pressure Settings
- **Enable Pressure Control:** Whether to use the pressure feature of this device. On by default.
- **Pressure Mode:** Determines how the pressure will be set.
	- **Cycle:** Gradually build up and release pressure over a fixed duration.
	- **Stroke Length:** Longer strokes = more pressure.
	- **Stroke Speed:** Faster strokes = more pressure.
- **Pressure Cycle Length (seconds):** If the Pressure Mode is set to Cycle, determines the length of a buildup-release cycle in seconds.

## Contributing
PRs for onboarding new games are welcome. The process is relatively simple and requires barely any coding. See the PlayHome implementation for reference. PRs for supporting new device types are also welcome.

This mod is provided free of charge, but I do accept donations. If you'd like to boost my morale, please check me out on [Patreon].

### Developers
Sauceke   •   nhydock   •   hogefugamoga   •   RPKU   •   andama777 (JP translation)

### Patrons
[ManlyMarco]   •   Aftercurve   •   AkronusWings   •   Ambicatus   •   Andrew Hall   •   AstralClock   •   Benos Hentai   •   boaz   •   Bri   •   cat tail   •   CBN ヴい   •   Ceruleon   •   CROM   •   Daniel   •   EPTG   •   er er   •   Flan   •   funnychicken   •   Gabbelgu   •   gold25   •   GOU YOSIHIRO   •   Greg   •   hiro   •   Ior1yagami   •   Kai Yami   •   KTKT   •   kuni   •   Laneo   •   mokochurin   •   Nemi   •   nppon   •   PhazR   •   Phil   •   prepare55   •   real name   •   rolandmitch   •   RP君   •   SavagePastry   •   Shakes   •   Taibe   •   tanu   •   TO   •   Tom   •   TrashTaste   •   ttrs   •   tutinoko   •   unitora   •   uruurian   •   Wel Adunno   •   yamada tarou   •   Zesty Cucumber   •   Zijian Wang   •   しゃどみん   •   シルバー   •   ふ   •   一太 川崎   •   優希 岩永   •   哲慶 宗   •   国崎往人   •   将也 三田   •   洋 冨岡   •   猛 羽場   •   终晓   •   郁弥 中村   •   闇《YAMI》   •   高島　渉

## Acknowledgements
This mod would not have been possible without the [BepInEx] plugin framework and, of course, the [Buttplug.io] project.

<!-- badges -->
[CI Badge]: https://github.com/Sauceke/LoveMachine/actions/workflows/commit.yml/badge.svg
[Downloads Badge]: https://img.shields.io/github/downloads/Sauceke/LoveMachine/total
[Patreon Badge]: https://shields.io/badge/patreon-grey?logo=patreon
[Download Button]: https://img.shields.io/badge/%E2%87%93_Download-blue?style=for-the-badge
[Demo Button]: https://img.shields.io/badge/%E2%96%B6_Watch_Demo_(NSFW)-pink?style=for-the-badge

<!-- own links -->
[installer]: https://github.com/Sauceke/LoveMachine/releases/latest/download/LoveMachineInstaller.exe
[LoveMachine.Experiments]: https://sauceke.github.io/LoveMachine.Experiments
[Hotdog]: https://sauceke.github.io/hotdog
[Patreon]: https://www.patreon.com/sauceke
[Demo video]: https://www.erome.com/a/DhT7BF4B

<!-- sponsored game links -->
[Custom Order Maid 3D 2]: https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ011538.html/?locale=en_US
[Holy Knight Ricca]: https://www.dlsite.com/maniax/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/RJ363824.html/?locale=en_US
[Honey Select 2]: https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ013722.html/?locale=en_US
[Houkago Rinkan Chuudoku]: https://www.dlsite.com/maniax/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/RJ189924.html/?locale=en_US
[Incubus 2: Camlann]: https://www.dlsite.com/maniax/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/RJ321625.html/?locale=en_US
[Insult Order]: https://www.dlsite.com/maniax/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/RJ220246.html/?locale=en_US
[RoomGirl]: https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ015465.html/?locale=en_US
[Koikatsu Sunshine]: https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ015724.html/?locale=en_US
[Secrossphere]: https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ015728.html/?locale=en_US
[PlayHome]: https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ015713.html/?locale=en_US
[Koikatsu]: https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ015719.html/?locale=en_US
[Koi-Koi VR: Love Blossoms]: https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ01000460.html/?locale=en_US

<!-- sponsored sex toy links -->
[The Handy]: https://www.thehandy.com/?ref=saucekebenfield&utm_source=saucekebenfield&utm_medium=affiliate&utm_campaign=The+Handy+Affiliate+program
[Lovense Calor]: https://www.lovense.com/r/vu65q6
[Lovense Gush]: https://www.lovense.com/r/f7lki7
[Lovense Max 2]: https://www.lovense.com/r/k8bbja
[Lovense Diamo]: https://www.lovense.com/r/54xpc7
[Lovense Domi 2]: https://www.lovense.com/r/77i51d

<!-- other links -->
[ManlyMarco]: https://github.com/ManlyMarco
[Buttplug.io]: https://github.com/buttplugio/buttplug
[Intiface Central]: https://intiface.com/central
[BepInEx]: https://github.com/BepInEx
[AGHVR]: https://github.com/Eusth/AGHVR
[IOVR]: https://github.com/Eusth/IOVR
[Our Apartment]: https://www.patreon.com/momoirosoftware
[Koikatsu Party]: https://store.steampowered.com/app/1073440/__Koikatsu_Party/
[VR Kanojo]: http://www.illusion.jp/preview/vrkanojo/index_en.php
[Last Evil]: https://store.steampowered.com/app/823910/last_evil/
[Succubus Cafe]: https://store.steampowered.com/app/1520500/Succubus_Cafe/
