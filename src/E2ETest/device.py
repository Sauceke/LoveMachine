import abc
import asyncio
import dataclasses
import json
import re
import threading
import time
import websockets


@dataclasses.dataclass
class LinearCmd:
    position: float
    millis: int


class SimulatedDevice:

    def __init__(self, port):
        self._thread = threading.Thread(target=lambda: asyncio.run(self._connect(port)))
        self._thread.start()

    async def _connect(self, port):
        async with websockets.connect(uri=f"ws://127.0.0.1:{port}", ping_interval=0.5) as ws:
            await self.listen(ws)

    @abc.abstractmethod
    async def listen(self, ws):
        pass


class LovenseNora(SimulatedDevice):

    async def listen(self, ws):
        self.vibrate_cmd_log = {}
        self.rotate_cmd_log = {}
        self.battery_query_received = False
        address = "111111111111"
        await ws.send(json.dumps({"identifier": "LVSDevice", "address": address, "version": 0}))
        while True:
            packet: str = (await ws.recv()).decode("utf-8")
            if packet.startswith("DeviceType;"):
                await ws.send(bytes(f"A:{address}:10", "utf-8"))
            elif packet.startswith("Vibrate:"):
                if packet == "Vibrate:0;" and len(self.vibrate_cmd_log) == 0:
                    # only start recording from the first nonzero command,
                    # ignore StopDeviceCmd messages
                    continue
                self.vibrate_cmd_log[time.time()] = packet
            elif packet.startswith("Rotate:"):
                if packet == "Rotate:0;" and len(self.rotate_cmd_log) == 0:
                    continue
                self.rotate_cmd_log[time.time()] = packet
            elif packet.startswith("Battery"):
                self.battery_query_received = True
                await ws.send(bytes("90;", "utf-8"))


class OSR2(SimulatedDevice):

    async def listen(self, ws):
        self.linear_cmd_log = {}
        address = "222222222222"
        await ws.send(json.dumps({"identifier": "TCodeDevice", "address": address, "version": 0}))
        while True:
            packet: str = (await ws.recv()).decode("utf-8")
            match_linear = re.search(r"L(\d+)I(\d+)", packet)
            if match_linear is not None:
                position = int(match_linear.group(1)) / 100
                millis = int(match_linear.group(2))
                self.linear_cmd_log[time.time()] = LinearCmd(position, millis)


class LovenseSexMachine(SimulatedDevice):

    async def listen(self, ws):
        self.oscillate_cmd_log = {}
        self.battery_query_received = False
        address = "333333333333"
        await ws.send(json.dumps({"identifier": "LVSDevice", "address": address, "version": 0}))
        while True:
            packet: str = (await ws.recv()).decode("utf-8")
            match_oscillate = re.search(r"Vibrate:(\d+);", packet)
            if packet.startswith("DeviceType;"):
                await ws.send(bytes(f"F:{address}:10", "utf-8"))
            elif match_oscillate is not None:
                speed = int(match_oscillate.group(1)) / 20.0
                if speed == 0 and len(self.oscillate_cmd_log) == 0:
                    # ignore StopDeviceCmd messages
                    continue
                self.oscillate_cmd_log[time.time()] = speed
            elif packet.startswith("Battery"):
                self.battery_query_received = True
                await ws.send(bytes("90;", "utf-8"))
