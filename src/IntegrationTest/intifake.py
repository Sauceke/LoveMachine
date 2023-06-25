import asyncio
import json
import threading
import time
import websockets

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

class Intifake:
    """Fake Intiface server using example responses taken straight from the Buttplug docs."""

    def __init__(self):
        # Dict of linear commands keyed with the time each command was received in epoch seconds
        self.linear_commands = {}
        # Dict of vibrate commands keyed with the time each command was received in epoch seconds
        self.vibrate_commands = {}

    async def _handle(self, websocket):
        while True:
            try:
                message = await websocket.recv()
            except:
                print("recv error, assuming connection closed")
                self._stop_handle.set_result(0)
                continue
            obj = json.loads(message)[0]
            if "RequestServerInfo" in obj:
                await websocket.send(server_info)
                continue
            if "RequestDeviceList" in obj:
                await websocket.send(device_list)
                continue
            if "LinearCmd" in obj:
                self.linear_commands[time.time()] = obj["LinearCmd"]
                continue
            if "ScalarCmd" in obj:
                self.vibrate_commands[time.time()] = obj["ScalarCmd"]
                continue

    async def _run_loop(self):
        self._stop_handle = asyncio.Future()
        server = await websockets.serve(self._handle, host="localhost", port=12345, ping_timeout=600)
        await self._stop_handle
        try:
            await server.close()
        except:
            pass

    def _run_fg(self):
        asyncio.run(self._run_loop())

    def start(self):
        self._server_thread = threading.Thread(target=self._run_fg)
        self._server_thread.start()

    def stop(self):
        try:
            self._stop_handle.set_result(0)
            self._server_thread.join()
        except:
            pass
