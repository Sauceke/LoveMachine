# Troubleshooting

Before anything else, make sure you are using the latest version of both Intiface Central and
LoveMachine.

## Device is not doing anything

1. Make sure you can control your device from Intiface by following this guide:
   https://docs.intiface.com/docs/intiface-central/quickstart. If not even that works, try the
   troubleshooting guide on the same site, or try reaching out on the [Buttplug Forum].
2. Check if BepInEx is installed correctly. There should be a BepInEx\LogOutput.log file or an
   output_log.txt file in your game folder. If you delete it, it should appear again when you start
   the game. If it doesn't, then BepInEx isn't installed correctly. Follow the installation steps
   here: https://docs.bepinex.dev/articles/user_guide/installation/index.html
3. Make sure there are no other plugin frameworks that could interfere with BepInEx (IPA, Sybaris,
   ReiPatcher). If there is an HF patch available for your game, installing it should get rid of the
   conflicting plugin frameworks.
4. Do you have any mods installed besides LoveMachine? If so, try disabling them one by one (most
   easily done by moving them to a folder outside of `BepInEx/plugins`).
5. Do you have any DLCs installed? If you do, see if you can get LoveMachine working on a vanilla
   installation of the game first, to rule out the DLCs being the cause of the problem.
6. **If one of the previous two steps helped, please open an [issue] and mention which mod or DLC
   caused the problem.**
7. Check the Intiface status. If it says "waiting for client" while the game is running, refer to
   [this section](#lovemachine-is-not-connecting-to-intiface).
8. Have you pressed Space while in the game? The Space key is an emergency brake that stops all
   devices immediately. To restart them, press F8. You can remap both of these key bindings in the
   Plugin Settings.

## LoveMachine is not connecting to Intiface

1. Make sure LoveMachine is installed at all. Check your BepInEx\LogOutput.log or output_log.txt
   file. There should be at the very least a "Connecting to Intiface server" message in it. If there
   isn't, refer to [this section](#lovemachine-doesnt-even-start).
2. Make sure the WebSocket host and port in the Plugin Settings are set to the same address as the
   Server Address in Intiface.
3. Make sure nothing else is using the same port as Intiface (12345 by default).
4. If using a VPN, make sure it's not messing with your local ports (apparently, that can happen
   sometimes).

## LoveMachine doesn't even start

1. Try reinstalling LoveMachine using the latest installer.
2. Try installing the vanilla version of the game in a different location, then installing
   LoveMachine on top of that using the latest installer.
3. Has the game had any updates recently? If so, try installing an earlier version of the game.

## Bluetooth device is out of sync / lags behind the game

1. Make sure your Bluetooth adapter supports Bluetooth 5.0 and EDR.
2. Make sure your PC is actually using **that** Bluetooth adapter, and not a different one. If there
   is an on-board Bluetooth adapter with lower specs, disable it in your Device Manager.
3. Move your device closer to your PC and clear the path from obstructions.
4. Try controlling your device directly from Intiface to check whether the lag is caused by
   LoveMachine or something else.
5. If using a stroker, try turning off Smooth Stroking in the Plugin Settings.
6. If nothing else works, try decreasing the Updates Per Second value for the device in the Plugin
   Settings.

## Oscillating toy (Hismith/Svakom/Lovense/etc.) is out of sync

1. There is no fix, oscillating toys cannot be accurately synced.
2. If the oscillating speed is a lot different from the in-game speed, adjust the RPM Range in the
   Plugin Settings.

## If nothing in this guide helps

Please open an [issue].

[issue]:
  https://github.com/Sauceke/LoveMachine/issues/new?assignees=&labels=&projects=&template=technical-problem.md&title=
[Buttplug Forum]: https://discuss.buttplug.io/
