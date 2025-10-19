*** Settings ***
Name              Secrossphere Demo Test Suite
Documentation     End-to-end tests of LoveMachine.SCS running on
...               Secrossphere demo, with simulated websocket
...               devices connected to Intiface Engine.
Library           LoveMachineLibrary.py
Library           GameLibrary.py
Suite Setup       Play The Game
Suite Teardown    Clean Up

*** Variables ***
${Game URL}        https://trial.dlsite.com/professional/VJ016000/VJ015728_trial.zip
${BepInEx URL}     https://github.com/BepInEx/BepInEx/releases/download/v5.4.22/BepInEx_x86_5.4.22.0.zip

*** Test Cases ***
Linear Command Count
    Number Of Linear Commands Should Be At Least       ${30}

Linear Command Timing
    Time Between Linear Commands Should Be About       400 ms

Linear Command Position
    Positions Of Linear Commands Should Alternate

Linear Command Duration
    Durations Of Linear Commands Should Be About       400 ms

Vibrate Command Count
    Number Of Vibrate Commands Should Be At Least      ${100}

Vibrate Command Timing
    Time Between Vibrate Commands Should Be About      100 ms

Rotate Command Count
    Number Of Rotate Commands Should Be At Least       ${30}

Rotate Command Timing
    Time Between Rotate Commands Should Be About       400 ms

Oscillate Command Count
    Number Of Oscillate Commands Should Be At Least    ${1}

Oscillate Command Speed
    Speeds Of Oscillate Commands Should All Be         ${0.3}

Battery Level
    Battery Level Of Vibrator Should Have Been Read
    Battery Level Of Oscillator Should Have Been Read

Kill Switch
    WHEN Press Key                                     space
    AND Sleep                                          5 seconds
    THEN No Command Should Have Arrived In The Last    5 seconds

*** Keywords ***
Play The Game
    Install Secrossphere Demo
    Download Intiface Engine
    Start Intiface Engine
    Connect Lovense Nora
    Connect Lovense Sex Machine
    Connect OSR2
    Launch Game    bin/scs/Trial.exe
    Sleep          30 seconds    let the game load
    Start H Scene
    Sleep          30 seconds    let the h-scene run

Install Secrossphere Demo
    Download ZIP    ${Game URL}                  bin/scs-dl
    Copy Content    bin/scs-dl/*                 bin/scs
    Download ZIP    ${BepInEx URL}               bin/bepinex
    Copy Content    bin/bepinex                  bin/scs
    Copy Content    ../LoveMachine.SCS/tweaks    bin/scs
    Copy Content    ../bin/LoveMachine.SCS       bin/scs
    Copy Content    scs-config                   bin/scs

Start H Scene
    Press Key         s
    Press Key         enter
    Sleep             5 seconds    let the dialog load
    Repeat Keyword    14 times     Left Click

Clean Up
    Close Game
    Close Intiface Engine
