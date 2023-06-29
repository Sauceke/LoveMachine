import asyncio
import json
import websockets
import time
import abc
import threading
import re

class LinearCmd:
  
  def __init__(self, position, millis):
    self.position = position
    self.millis = millis


class SimulatedDevice:

  def __init__(self, port):
    self._thread = threading.Thread(target=lambda: asyncio.run(self._connect(port)))
    self._thread.start()

  async def _connect(self, port):
    async with websockets.connect(uri = f"ws://127.0.0.1:{port}", ping_interval = 0.5) as ws:
      await self.listen(ws)
  
  @abc.abstractmethod
  async def listen(self, ws):
    pass


class LovenseVibratorDevice(SimulatedDevice):

  async def listen(self, ws):
    self.vibrate_cmd_log = {}
    self.battery_query_received = False
    address = "123456789ABC"
    await ws.send(json.dumps({ "identifier": "LVSDevice", "address": address, "version": 0}))
    while True:
        packet: str = (await ws.recv()).decode("utf-8")
        if packet.startswith("DeviceType;"):
          await ws.send(bytes(f"Z:{address}:10", "utf-8"))
        elif packet.startswith("Vibrate:"):
          self.vibrate_cmd_log[time.time()] = packet
        elif packet.startswith("Battery"):
          self.battery_query_received = True
          await ws.send(bytes("90;", "utf-8"))


class TCodeStrokerDevice(SimulatedDevice):

  async def listen(self, ws):
    self.linear_cmd_log = {}
    address = "CBA987654321"
    await ws.send(json.dumps({ "identifier": "TCodeDevice", "address": address, "version": 0}))
    while True:
        packet: str = (await ws.recv()).decode("utf-8")
        match_linear = re.search(r"L(\d+)I(\d+)", packet)
        if match_linear is not None:
          position = int(match_linear.group(1)) / 100
          millis = int(match_linear.group(2))
          self.linear_cmd_log[time.time()] = LinearCmd(position, millis)
