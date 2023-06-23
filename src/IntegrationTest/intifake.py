import concurrent
import json
import asyncio
import time
import websockets
import threading

# Fake Intiface server using example responses taken straight from the Buttplug docs.

server_info = """
[
    {
        "ServerInfo": {
            "Id": 1,
            "ServerName": "Test Server",
            "MessageVersion": 1,
            "MaxPingTime": 100
        }
    }
]
"""

device_list = """
[
    {
        "DeviceList": {
            "Id": 1,
            "Devices": [
                {
                    "DeviceName": "Test Vibrator",
                    "DeviceIndex": 0,
                    "DeviceMessages": {
                        "ScalarCmd": [
                            {
                                "StepCount": 20,
                                "FeatureDescriptor": "Clitoral Stimulator",
                                "ActuatorType": "Vibrate"
                            },
                            {
                                "StepCount": 20,
                                "FeatureDescriptor": "Insertable Vibrator",
                                "ActuatorType": "Vibrate"
                            }
                        ],
                        "StopDeviceCmd": {}
                    }
                },
                {
                    "DeviceName": "Test Stroker",
                    "DeviceIndex": 1,
                    "DeviceMessageTimingGap": 100,
                    "DeviceDisplayName": "User set name",
                    "DeviceMessages": {
                        "LinearCmd": [{
                            "StepCount": 100,
                            "FeatureDescriptor": "Stroker",
                            "ActuatorType": "Linear"
                        }],
                        "StopDeviceCmd": {}
                    }
                }
            ]
        }
    }
]
"""

linear_commands = {}
vibrate_commands = {}

async def handle(websocket):
    while True:
        try:
            message = await websocket.recv()
        except Exception as e:
            print(e)
            print("recv error, assuming connection closed")
            stop_handle.set_result(0)
            continue
        obj = json.loads(message)[0]
        if "RequestServerInfo" in obj:
            await websocket.send(server_info)
            continue
        if "RequestDeviceList" in obj:
            await websocket.send(device_list)
            continue
        if "LinearCmd" in obj:
            global linear_commands
            linear_commands[time.time()] = message
            continue
        if "ScalarCmd" in obj:
            global vibrate_commands
            vibrate_commands[time.time()] = message
            continue

async def run_loop():
    global stop_handle
    stop_handle = asyncio.Future()
    server = await websockets.serve(handle, host="localhost", port=12345, ping_timeout=600)
    await stop_handle
    try:
        await server.close()
    except:
        pass

def run_fg():
    asyncio.run(run_loop())

def start():
    global server_thread
    server_thread = threading.Thread(target = run_fg)
    server_thread.start()

def stop():
    try:
        global stop_handle
        stop_handle.set_result(0)
        global server_thread
        server_thread.join()
    except:
        pass
