[English](README.md) | [日本語](README-jp.md)

# LoveMachine

[![QA][CI Badge]](#) [![Download][Downloads Badge]][installer] [![Patreon][Patreon Badge]][Patreon]

[![Download][Download Button]][installer] &nbsp; [![Watch Demo (NSFW)][Demo Button]][Demo video]

Adds support for [some computer-controlled sex toys](#supported-devices) in the following games:

| Title                       | Developer     | 🥽 VR                         | 🖥 Desktop |
| --------------------------- | ------------- | ----------------------------- | --------- |
| [AI-deal-Rays]              | Riez-ON       |                               | ✓         |
| AI Shoujo                   | Illusion      | ✓ <sup>with [AISVR]</sup>     | ✓         |
| [Custom Order Maid 3D 2]    | Kiss          | ✓                             | ✓         |
| [Datsui Janken]             | Visionary     |                               | ✓         |
| Emotion Creators            | Illusion      | ✓ <sup>with [EC_VR]</sup>     | ✓         |
| [Holy Knight Ricca]         | Mogurasoft    |                               | ✓         |
| [HoneyCome] & Digital Craft | Illgames      | ✓ <sup>in Digital Craft</sup> | ✓         |
| Honey Select                | Illusion      |                               | ✓         |
| Honey Select 2              | Illusion      | ✓                             | ✓         |
| [Houkago Rinkan Chuudoku]   | Miconisomi    | ✓ <sup>with [AGHVR]</sup>     | ✓         |
| [Incubus 2: Camlann]        | Tanpakusitsu  |                               | ✓         |
| [Insult Order]              | Miconisomi    | ✓ <sup>with [IOVR]</sup>      | ✓         |
| Koikatsu                    | Illusion      | ✓                             | ✓         |
| Koikatsu Party              | Illusion      | ✓                             | ✓         |
| Koikatsu Sunshine           | Illusion      | ✓                             | ✓         |
| [Koi-Koi VR: Love Blossoms] | Apricot Heart | ✓                             | ✓         |
| [Last Evil]                 | Flametorch    |                               | ✓         |
| [Oedo Trigger]              | CQC Software  | ✓ <sup>sold separately</sup>  | ✓         |
| PlayHome                    | Illusion      | ✓                             | ✓         |
| RoomGirl                    | Illusion      |                               | ✓         |
| Secrossphere                | Illusion      |                               | ✓         |
| [Sexaroid Girl]             | Daminz        | ✓                             |           |
| [Solas City Heroes]         | MrZGames      |                               | ✓         |
| [Succubus Cafe]             | Migi Studio   |                               | ✓         |
| VR Kanojo                   | Illusion      | ✓                             |           |
| [Writhing Play]             | Robi          | ✓                             | ✓         |

The following early access games are also supported, but compatibility with later versions of them
is not guaranteed.

| Title                      | Developer          | Supported Version | Plugin                                                   |
| -------------------------- | ------------------ | ----------------- | -------------------------------------------------------- |
| [Our Apartment]            | Momoiro Software   | 0.5.3.a           | Available in the [installer]                             |
| [Melty Night VR]           | Cauchemar          | 0.5.5             | [Patreon post][Patreon-MNVR], or build the `mnvr` branch |
| [Orc Massage]              | TorchEntertainment | July 11, 2023     | [Patreon post][Patreon-OM], or build the `om` branch     |
| [Summer In Heat]           | Miconisomi         | 1.00              | [Free patreon post][Patreon-SIH]                         |
| [Summer Vacation Scramble] | Illgames           | 1.0.0             | [Free patreon post][Patreon-SVS]                         |
| [Sex Formula]              | Migi Studio        | 1.3.0             | [Patreon post][Patreon-SF], or build the `sf` branch     |
| [Gals Collector]           | Studio Tris        | 1.04              | [Free patreon post][Patreon-GC]                          |

## Supported devices

LoveMachine connects to adult toys through the Buttplug protocol, which supports over 200 devices.
Among those, LoveMachine can recognize **linear** (moving back-and-forth), **vibrating**,
**rotating** and **tightening** sex toys.

Some of the devices that have been confirmed to work well with the mod:

Strokers

- [The Handy]
- [Kiiroo KEON]
- OSR2

Vibrators

- [Lovense Gush]
- [Lovense Max 2]
- [Lovense Diamo]
- [Lovense Domi 2]
- [Lovense Calor]
- The Xbox gamepad

Rotators

- Vorze A10 Cyclone

Oscillators

- [Lovense Gravity]
- [Lovense Solace]

ℹ️ **Every supported device works with every game. No exceptions.**

⚠ **The devices listed under Oscillators have no positional feedback. They don't know which way is
up and down, so they can't accurately sync to in-game characters. If you want to buy a device that
can, you're looking for something under Strokers.**

The [LoveMachine.Experiments] plugin also adds experimental support for two depth sensing devices
([Lovense Calor] and the [Hotdog]) to Koikatsu and Koikatsu Sunshine.

## Installation

Download and run the [installer]. If you encounter the "Windows protected your PC" message, click
More info > Run anyway.

[Intiface Central] must also be installed.

## How to use

1. Open Intiface Central.
1. Click on the big Play button.
1. Turn on the device you want to use.
1. Start the game.

The Space key acts as a kill switch for all devices while in-game. To reactivate your devices, press
F8. Both of these key bindings can be modified under Plugin Settings > LoveMachine > Kill Switch
Settings.

⚠ In certain games, the kill switch may not work if the BepInEx console is open while playing in VR,
because it can steal focus from the game window. It is recommended to disable the console.

If you need further help in using the plugin, check the [Troubleshooting](troubleshooting.md) page
or open an [issue].

If you found this project useful, please give it a ⭐.

## How it works, limitations

Whenever a new animation loop starts, LoveMachine records the relative positions of certain bones
for one cycle, then it tries to guess which bones are the most likely to be involved in the action
(e.g. a penis and a mouth). More often than not, it guesses correctly; when it doesn't, you can
manually select which bone of which character to track in the Plugin Settings. You can even select a
different bone for each device to reenact more complex scenes.

After one cycle of learning, the plugin translates the relative movement of the guessed/selected
bones into something that the device can perform (e.g. axial movement for strokers, or rotating
back-and-forth for rotators).

As the whole thing is based on bone positions, this will only work for reasonably sized and
proportioned characters.

## Configuration

⚠ IL2CPP games (RoomGirl, Holy Knight Ricca) are not compatible with ConfigurationManager at the
moment. If you want to change the settings in those games, you can edit the configuration file
(`BepInEx\config\Sauceke.LoveMachine.IL2CPP.cfg`) in Notepad.

In Plugin Settings > LoveMachine, you can set the following parameters:

### Animation Settings (KK/KKS only)

- **Track Animation Blending:** H-Scene animations in Koikatsu and KKS shift slowly back and forth
  between two variants of the same animation. The two variants sometimes have different stroke
  patterns. This setting allows you to track both variants, which makes animation tracking more
  precise, but may also result in some abrupt changes mid-stroke. Turned on by default.

### Core Settings

- **POV:** Which character's point of view should be simulated. Possible values:
  - **Balanced:** Replicates relative movement. Should work for most players, most of the time.
  - **Male:** Male bottom POV. Only replicates the female character's movements.
  - **Female:** Female bottom POV. Only replicates the male character's movements.

### Device List

This is where all your devices connected to Intiface are listed.

- **Connect:** Connect or reconnect to the Intiface server.
- **Scan:** Scan for devices.

General device settings (all devices):

- **Group Role:** Which girl the device is assigned to in a group scene. This also affects scenes
  that are not group scenes, e.g. if a device is assigned to second girl, and there is only one girl
  in the scene, it will not be activated at all.
- **Body Part:** Selects the body part that will be tracked by the device. Defaults to Auto (which
  means it will find the one closest to the player's balls). Can be used to re-enact TJ/FJ with
  alternating movement using two devices. In Koikatsu and KKS, it also tracks fondling/fingering
  movements.
- **Latency (milliseconds):** Latency of sex toys is usually negligible, but if you're experiencing
  any noticeable delay between your display and your device, use this setting to correct it. There's
  no way to calibrate this, so you'll have to experiment.
- **Updates per second:** How often to send commands to this device. BLE devices can usually handle
  about 10-20 commands per second.

Stroker settings:

- **Max Strokes (per minute):** The maximum speed your stroker is capable of at 100% stroke length.
- **Stroke Zone / Slow:** The range of the stroking motion when going slow. 0% is the bottom, 100%
  is the top.
- **Stroke Zone / Fast:** The range of the stroking motion when going fast. 0% is the bottom, 100%
  is the top.
- **Smooth Stroking:** Makes the stroking movement less robotic, but not all strokers can handle
  this. Known to work well on Handy and OSR2 devices. Turned off by default.

Vibrator settings:

- **Intensity Range:** Minimum and maximum vibration intensity allowed for this device. 0% = no
  vibration, 100% = full strength.
- **Vibration Pattern:** The waveform of the vibration intensity. Available values are Sine,
  Triangle, Saw, Pulse, Constant, and Custom.
- **Custom Pattern:** Available if Vibration Pattern is set to Custom. You can set the vibration
  intensity curve using the sliders.

Oscillator settings:

- **RPM Range:** The minimum and maximum rotations per minute this device is capable of.

Pressure settings:

- **Pressure Range:** Minimum and maximum pressure allowed on this device, in percentages.
- **Pressure Update Interval (seconds)** How much time it takes for this device to change pressure,
  in seconds. Defaults to 5.

You may also want to:

- **Save device assignments:** If enabled, the Threesome Role and Body Part attributes will be saved
  for all devices. Disabled by default.

### Intiface Settings

- **WebSocket host:** The URL of the host Intiface is running on. Should be `ws://127.0.0.1` unless
  it's running on a remote machine.
- **WebSocket port:** The port Intiface is listening on. Usually `12345`.
- **Reconnect Backoff Time (seconds):** Waiting time between attempts to connect if the connection
  was lost or not made.

### Kill Switch Settings

Safety measure to avoid hurting yourself if the sex gets too rough or something goes wrong. By
default, pressing Spacebar will immediately stop all connected devices.

- **Emergency Stop Key Binding:** Sets the keystroke for activating the kill switch (Space by
  default).
- **Resume Key Binding:** Sets the keystroke for deactivating the kill switch (F8 by default).

### Stroker Settings

- **Stroke Length Realism:** How much the stroke length should match the animation. 0% means every
  stroke will use the full available length. 100% means every stroke will be scaled to its in-game
  length.
- **Hard Sex Intensity:** How fast your stroker will fall during hard sex animations. 100% is twice
  as fast as 0%. I'm not responsible for any injuries that may occur due to the use of LoveMachine.
- **Orgasm Depth:** The position of the stroker during orgasm.
- **Orgasm Shaking Frequency:** How many strokes to do per second during orgasm.

### Rotator Settings

- **Rotation Speed Ratio:** The speed ratio for rotation. 0% is no rotation, 100% is full speed
  rotation. Default is 50%.
- **Rotation Direction Change Chance:** The direction of rotation changes with the probability of
  this setting. Default is 30%.

### Oscillation Settings

- **RPM limit:** Maximum allowed rotations per minute for any device. Default is 300.

### Pressure Settings

- **Enable Pressure Control:** Whether to use the pressure feature of this device. On by default.
- **Pressure Mode:** Determines how the pressure will be set.
  - **Cycle:** Gradually build up and release pressure over a fixed duration.
  - **Stroke Length:** Longer strokes = more pressure.
  - **Stroke Speed:** Faster strokes = more pressure.
- **Pressure Cycle Length (seconds):** If the Pressure Mode is set to Cycle, determines the length
  of a buildup-release cycle in seconds.

## Contributing

PRs for onboarding new games are welcome. The process is relatively simple and requires barely any
coding. See the PlayHome implementation for reference. PRs for supporting new device types are also
welcome.

This mod is provided free of charge, but I do accept donations. If you'd like to boost my morale,
please check me out on [Patreon].

## Acknowledgements

Thanks to nhydock, hogefugamoga, RPKU, and andama777 for contributing to the project, and to my
Patreon members for their generous support.

This mod would not have been possible without the [BepInEx] plugin framework and, of course, the
[Buttplug.io] project.

<!-- badges -->

[CI Badge]: https://github.com/Sauceke/LoveMachine/actions/workflows/qa.yml/badge.svg
[Downloads Badge]: https://img.shields.io/github/downloads/Sauceke/LoveMachine/total
[Patreon Badge]: https://shields.io/badge/patreon-grey?logo=patreon
[Download Button]: https://img.shields.io/badge/%E2%87%93_Download-blue?style=for-the-badge
[Demo Button]: https://img.shields.io/badge/%E2%96%B6_Watch_Demo_(NSFW)-pink?style=for-the-badge

<!-- own links -->

[installer]:
  https://github.com/Sauceke/LoveMachine/releases/latest/download/LoveMachineInstaller.exe
[LoveMachine.Experiments]: https://sauceke.github.io/LoveMachine.Experiments
[Hotdog]: https://sauceke.github.io/hotdog
[Patreon]: https://www.patreon.com/sauceke
[Patreon-MNVR]: https://www.patreon.com/posts/lovemachine-for-105156790
[Patreon-OM]: https://www.patreon.com/posts/lovemachine-for-105156790
[Patreon-SIH]: https://www.patreon.com/posts/lovemachine-for-121132226
[Patreon-SVS]: https://www.patreon.com/posts/lovemachine-for-111228062
[Patreon-SF]: https://www.patreon.com/posts/lovemachine-for-116761160
[Patreon-GC]: https://www.patreon.com/posts/lovemachine-for-119312328
[Demo video]: https://www.erome.com/a/f2XKHJ1I
[issue]: https://github.com/Sauceke/LoveMachine/issues/new?template=technical-problem.md

<!-- sponsored game links -->

[Custom Order Maid 3D 2]:
  https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ011538.html/?locale=en_US
[Holy Knight Ricca]:
  https://www.dlsite.com/maniax/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/RJ363824.html/?locale=en_US
[Houkago Rinkan Chuudoku]:
  https://www.dlsite.com/maniax/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/RJ189924.html/?locale=en_US
[Incubus 2: Camlann]:
  https://www.dlsite.com/maniax/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/RJ321625.html/?locale=en_US
[Insult Order]:
  https://www.dlsite.com/maniax/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/RJ220246.html/?locale=en_US
[Koi-Koi VR: Love Blossoms]:
  https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ01000460.html/?locale=en_US
[Oedo Trigger]:
  https://www.dlsite.com/maniax/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/RJ439205.html/?locale=en_US
[HoneyCome]:
  https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ01000785.html/?locale=en_US
[Datsui Janken]:
  https://www.dlsite.com/maniax/dlaf/=/t/s/link/work/aid/sauceke/locale/en_US/id/RJ435105.html/?locale=en_US
[Sexaroid Girl]:
  https://www.dlsite.com/maniax/dlaf/=/t/s/link/work/aid/sauceke/locale/en_US/id/RJ188839.html/?locale=en_US
[Writhing Play]:
  https://www.dlsite.com/maniax/dlaf/=/t/s/link/work/aid/sauceke/locale/en_US/id/RJ303936.html/?locale=en_US
[AI-deal-Rays]:
  https://www.dlsite.com/maniax/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/RJ406835.html/?locale=en_US
[Summer Vacation Scramble]:
  https://www.dlsite.com/pro/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/VJ01002420.html/?locale=en_US
[Gals Collector]:
  https://www.dlsite.com/maniax/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/RJ01285811.html/?locale=en_US
[Summer In Heat]:
  https://dlaf.jp/maniax/dlaf/=/t/n/link/work/aid/sauceke/locale/en_US/id/RJ365188.html/?locale=en_US

<!-- sponsored sex toy links -->

[The Handy]:
  https://www.thehandy.com/?ref=saucekebenfield&utm_source=saucekebenfield&utm_medium=affiliate&utm_campaign=The+Handy+Affiliate+program
[Kiiroo KEON]: https://feelrobotics.go2cloud.org/aff_c?offer_id=4&aff_id=1125&url_id=203
[Lovense Calor]: https://www.lovense.com/r/vu65q6
[Lovense Gush]: https://www.lovense.com/r/f7lki7
[Lovense Max 2]: https://www.lovense.com/r/k8bbja
[Lovense Diamo]: https://www.lovense.com/r/54xpc7
[Lovense Domi 2]: https://www.lovense.com/r/77i51d
[Lovense Gravity]: https://www.lovense.com/r/3n3jgv
[Lovense Solace]: https://www.lovense.com/r/t1ivev

<!-- other links -->

[ManlyMarco]: https://github.com/ManlyMarco
[Buttplug.io]: https://github.com/buttplugio/buttplug
[Intiface Central]: https://intiface.com/central
[BepInEx]: https://github.com/BepInEx
[AGHVR]: https://github.com/Eusth/AGHVR
[AISVR]: https://vr-erogamer.com/archives/665
[EC_VR]: https://yuki-portal.com/uploader/emotioncreator/52532/
[IOVR]: https://github.com/Eusth/IOVR
[Our Apartment]: https://www.patreon.com/momoirosoftware
[Last Evil]: https://store.steampowered.com/app/823910/last_evil/
[Succubus Cafe]: https://store.steampowered.com/app/1520500/Succubus_Cafe/
[Solas City Heroes]: https://store.steampowered.com/app/2060170/Solas_City_Heroes/
[Melty Night VR]: https://ci-en.dlsite.com/creator/3131
[Orc Massage]: https://store.steampowered.com/app/1129540/Orc_Massage/
[Sex Formula]: https://store.steampowered.com/app/2889660/Sex_Formula/
