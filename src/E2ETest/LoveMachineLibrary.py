import device
import os
import pynput
import requests
import robot
import shutil
import statistics
import subprocess
import time


root_path = "./bin/"
intiface_url = "https://github.com/intiface/intiface-engine/releases/download/v1.4.8/intiface-engine-win-x64-Release.zip"
wsdm_port = 54817


class LoveMachineLibrary:
    ROBOT_LIBRARY_SCOPE = "SUITE"

    def __init__(self):
        if not os.path.exists(root_path):
            os.makedirs(root_path)
        self._mouse = pynput.mouse.Controller()
        self._keyboard = pynput.keyboard.Controller()

    def _durations_should_be_about(self, durations_s, expected_str):
        expected_s = robot.libraries.DateTime.convert_time(expected_str)
        tolerance_s = 0.3
        tolerance_mean = 0.3
        tolerance_stdev_s = 0.1
        assert all(abs(actual_s - expected_s) < tolerance_s for actual_s in durations_s)
        assert abs(statistics.mean(durations_s) - expected_s) < expected_s * tolerance_mean
        assert statistics.stdev(durations_s) < tolerance_stdev_s

    def _timestamp_gaps_should_be_about(self, timestamps_s, expected_str):
        gaps = [tup[1] - tup[0] for tup in zip(timestamps_s[:-1], timestamps_s[1:])]
        self._durations_should_be_about(gaps, expected_str)
    
    def download_intiface_engine(self):
        intiface_zip_path = root_path + "intiface.zip"
        req = requests.get(intiface_url, allow_redirects=True)
        open(intiface_zip_path, 'wb').write(req.content)
        robot.api.logger.info("Downloaded Intiface Engine")
        shutil.unpack_archive(intiface_zip_path, root_path)
        robot.api.logger.info("Extracted Intiface Engine")

    def start_intiface_engine(self):
        args = [
            root_path + "intiface-engine.exe",
            "--websocket-port", "12345",
            "--use-device-websocket-server",
            "--device-websocket-server-port", str(wsdm_port),
            "--user-device-config-file", "./buttplug-user-device-config.json"
            ]
        self._intiface_process = subprocess.Popen(args)
        robot.api.logger.info("Started Intiface Engine")

    def close_intiface_engine(self):
        self._intiface_process.terminate()
        robot.api.logger.info("Closed Intiface Engine")

    def connect_osr2(self):
        self._stroker = device.OSR2(wsdm_port)

    def connect_lovense_nora(self):
        self._vibrator = device.LovenseNora(wsdm_port)
        self._rotator = self._vibrator
    
    def press_key(self, key):
        self._keyboard.tap(key if len(key) == 1 else pynput.keyboard.Key[key])
        time.sleep(1)

    def left_click(self):
        self._mouse.click(pynput.mouse.Button.left)
        time.sleep(1)

    def number_of_linear_commands_should_be_at_least(self, min):
        robot.api.logger.info("Captured linear commands: " + str(self._stroker.linear_cmd_log))
        assert len(self._stroker.linear_cmd_log) >= min
    
    def number_of_vibrate_commands_should_be_at_least(self, min):
        robot.api.logger.info("Captured vibrate commands: " + str(self._vibrator.vibrate_cmd_log))
        assert len(self._vibrator.vibrate_cmd_log) >= min

    def number_of_rotate_commands_should_be_at_least(self, min):
        robot.api.logger.info("Captured rotate commands: " + str(self._rotator.rotate_cmd_log))
        assert len(self._rotator.rotate_cmd_log) >= min

    def time_between_linear_commands_should_be_about(self, duration_str):
        # discard first command as it's not guaranteed to be aligned
        timestamps = sorted(self._stroker.linear_cmd_log.keys())[1:]
        self._timestamp_gaps_should_be_about(timestamps, duration_str)
        
    def time_between_vibrate_commands_should_be_about(self, duration_str):
        timestamps = sorted(self._vibrator.vibrate_cmd_log.keys())
        self._timestamp_gaps_should_be_about(timestamps, duration_str)

    def time_between_rotate_commands_should_be_about(self, duration_str):
        # first 2 commands not guaranteed to be aligned
        timestamps = sorted(self._rotator.rotate_cmd_log.keys())[2:]
        self._timestamp_gaps_should_be_about(timestamps, duration_str)

    def positions_of_linear_commands_should_alternate(self):
        commands_dict = self._stroker.linear_cmd_log
        timestamps = sorted(commands_dict.keys())
        positions = [commands_dict[t].position for t in timestamps]
        odd_positions = positions[1::2]
        even_positions = positions[::2]
        assert max(odd_positions) - min(odd_positions) < 0.2
        assert max(even_positions) - min(even_positions) < 0.2
        assert abs(max(odd_positions) - min(even_positions)) > 0.5
        assert abs(max(even_positions) - min(odd_positions)) > 0.5

    def durations_of_linear_commands_should_be_about(self, duration_str):
        commands_dict = self._stroker.linear_cmd_log
        # first command not guaranteed to be aligned
        timestamps = sorted(commands_dict.keys())[1:]
        durations_s = [commands_dict[t].millis / 1000 for t in timestamps]
        self._durations_should_be_about(durations_s, duration_str)

    def battery_level_of_vibrator_should_have_been_read(self):
        assert self._vibrator.battery_query_received

    def no_command_should_have_arrived_in_the_last(self, duration_str):
        seconds = robot.libraries.DateTime.convert_time(duration_str)
        end = time.time() - seconds
        assert all(timestamp < end for timestamp in self._stroker.linear_cmd_log.keys())
        assert all(timestamp < end for timestamp in self._vibrator.vibrate_cmd_log.keys())
