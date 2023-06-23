*** Settings ***
Library           LoveMachineLibrary.py
Suite Setup       Play The Game
Suite Teardown    Clean Up

*** Test Cases ***
Check Linear Commands
    Number Of Linear Commands Should Be At Least    ${10}
    Milliseconds Between Linear Commands Should Be About    ${400}
    Positions Of Linear Commands Should Alternate

Check Vibrate Commands
    Milliseconds Between Vibrate Commands Should Be About    ${100}

*** Keywords ***
Play The Game
    Download Secrossphere Demo
    Patch Secrossphere Demo
    Set Secrossphere Resolution    ${320}    ${240}    ${1}
    Use Secrossphere Config    ./scs-config
    Start Fake Intiface Server
    Start Secrossphere Demo
    Sleep    30 seconds
    Start H Scene In Secrossphere Demo
    Sleep    20 seconds

Clean Up
    Close Secrossphere Demo
    Stop Fake Intiface Server
    Sleep    5 seconds
