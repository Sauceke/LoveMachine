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