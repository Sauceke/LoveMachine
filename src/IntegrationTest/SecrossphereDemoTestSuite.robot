*** Settings ***
Name              Secrossphere Demo Test Suite
Documentation     Tests LoveMachine.SCS running on Secrossphere demo
...               with a fake Intiface server emulating a stroker and
...               a vibrator.
Library           LoveMachineLibrary.py
Suite Setup       Play The Game
Suite Teardown    Clean Up

*** Test Cases ***
Linear Command Count
    Number Of Linear Commands Should Be At Least             ${10}

Linear Command Timing
    Milliseconds Between Linear Commands Should Be About     ${400}

Linear Command Semantics
    Positions Of Linear Commands Should Alternate

Vibrate Command Count
    Number Of Vibrate Commands Should Be At Least            ${50}

Vibrate Command Timing
    Milliseconds Between Vibrate Commands Should Be About    ${100}

Kill Switch
    WHEN Press Space Bar
    AND Sleep                                                5 seconds
    THEN No Command Should Have Been Received In The Last    4 seconds

*** Keywords ***
Play The Game
    Download Secrossphere Demo
    Patch Secrossphere Demo
    Set Secrossphere Resolution           ${320}    ${240}    ${1}
    Use Secrossphere Config               ./scs-config
    Start Fake Intiface Server
    Start Secrossphere Demo
    Sleep                                 30 seconds
    Start H Scene In Secrossphere Demo
    Sleep                                 20 seconds

Clean Up
    Close Secrossphere Demo
    Stop Fake Intiface Server
    Sleep                                 5 seconds
    Delete Downloaded Files
