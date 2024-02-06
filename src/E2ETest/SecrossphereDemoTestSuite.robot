*** Settings ***
Name              Secrossphere Demo Test Suite
Documentation     End-to-end tests of LoveMachine.SCS running on
...               Secrossphere demo, with simulated websocket
...               devices connected to Intiface Engine.
Library           LoveMachineLibrary.py
Library           SecrossphereLibrary.py
Suite Setup       Play The Game
Suite Teardown    Clean Up

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

Battery Level
    Battery Level Of Vibrator Should Have Been Read

Kill Switch
    WHEN Press Key                                     space
    AND Sleep                                          5 seconds
    THEN No Command Should Have Arrived In The Last    5 seconds

*** Keywords ***
Play The Game
    Download Secrossphere Demo
    Patch Secrossphere Demo
    Set Secrossphere Resolution    ${320}        ${240}    ${1}
    Use Secrossphere Config        ./scs-config
    Download Intiface Engine
    Start Intiface Engine
    Connect Lovense Nora
    Connect OSR2
    Start Secrossphere Demo
    Sleep                          30 seconds    let the game load
    Start H Scene
    Sleep                          30 seconds    let the h-scene run

Start H Scene
    Press Key                      s
    Press Key                      enter
    Sleep                          5 seconds     let the dialog load
    Repeat Keyword                 14 times      Left Click

Clean Up
    Close Secrossphere Demo
    Close Intiface Engine
