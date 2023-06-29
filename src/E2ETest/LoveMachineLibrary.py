import device
import os
import pynput
import requests
import robot
import shutil
import subprocess
import time
import winreg
import zipfile

root_path = "./bin/"
bepinex32_url = "https://github.com/BepInEx/BepInEx/releases/download/v5.4.21/BepInEx_x86_5.4.21.0.zip"
intiface_url = "https://github.com/intiface/intiface-engine/releases/download/v1.4.0/intiface-engine-win-x64-Release.zip"
wsdm_port = 54817

scs_url = "https://trial.dlsite.com/professional/VJ016000/VJ015728_trial.zip"
scs_path = root_path + "scs/"
scs_tweaks_path = "../LoveMachine.SCS/tweaks"
scs_lovemachine_path = "../bin/LoveMachine.SCS"

class LoveMachineLibrary:
    ROBOT_LIBRARY_SCOPE = 'SUITE'

    def __init__(self):
        if not os.path.exists(root_path):
            os.makedirs(root_path)
        self._mouse = pynput.mouse.Controller()
        self._keyboard = pynput.keyboard.Controller()

    def _durations_should_be_about(self, durations_s, expected_str):
        expected_s = robot.libraries.DateTime.convert_time(expected_str)
        tolerance_s = 0.3
        tolerance_ratio = 0.3
        assert all(abs(actual_s - expected_s) < tolerance_s for actual_s in durations_s)
        assert abs(sum(durations_s) / len(durations_s) - expected_s) < expected_s * tolerance_ratio

    def _timestamp_gaps_should_be_about(self, timestamps_s, expected_str):
        gaps = [tup[1] - tup[0] for tup in zip(timestamps_s[:-1], timestamps_s[1:])]
        self._durations_should_be_about(gaps, expected_str)
    
    def download_intiface_engine(self):
        intiface_zip_path = root_path + "intiface.zip"
        req = requests.get(intiface_url, allow_redirects=True)
        open(intiface_zip_path, 'wb').write(req.content)
        robot.api.logger.info("Downloaded Intiface Engine")
        with zipfile.ZipFile(intiface_zip_path, "r", metadata_encoding="cp932") as intiface_zip:
            intiface_zip.extractall(root_path)
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

    def connect_stroker_to_intiface(self):
        self._stroker = device.TCodeStrokerDevice(wsdm_port)

    def connect_vibrator_to_intiface(self):
        self._vibrator = device.LovenseVibratorDevice(wsdm_port)
    
    def press_key(self, key):
        self._keyboard.tap(key if len(key) == 1 else pynput.keyboard.Key[key])
        time.sleep(1)

    def left_click(self):
        self._mouse.click(pynput.mouse.Button.left)
        time.sleep(1)

    def delete_downloaded_files(self):
        shutil.rmtree(root_path)

    def number_of_linear_commands_should_be_at_least(self, min):
        robot.api.logger.info("Captured linear commands: " + str(self._stroker.linear_cmd_log))
        assert len(self._stroker.linear_cmd_log) >= min
    
    def number_of_vibrate_commands_should_be_at_least(self, min):
        robot.api.logger.info("Captured vibrate commands: " + str(self._vibrator.vibrate_cmd_log))
        assert len(self._vibrator.vibrate_cmd_log) >= min

    def time_between_linear_commands_should_be_about(self, duration_str):
        # discard first command as it's not guaranteed to be aligned
        timestamps = sorted(self._stroker.linear_cmd_log.keys())[1:]
        self._timestamp_gaps_should_be_about(timestamps, duration_str)
        
    def time_between_vibrate_commands_should_be_about(self, duration_str):
        timestamps = sorted(self._vibrator.vibrate_cmd_log.keys())
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
        timestamps = sorted(commands_dict.keys())
        durations_s = [commands_dict[t].millis / 1000 for t in timestamps]
        self._durations_should_be_about(durations_s, duration_str)

    def battery_level_of_vibrator_should_have_been_read(self):
        assert self._vibrator.battery_query_received

    def no_command_should_have_arrived_in_the_last(self, duration_str):
        seconds = robot.libraries.DateTime.convert_time(duration_str)
        end = time.time() - seconds
        assert all(timestamp < end for timestamp in self._stroker.linear_cmd_log.keys())
        assert all(timestamp < end for timestamp in self._vibrator.vibrate_cmd_log.keys())

    def download_secrossphere_demo(self):
        robot.api.logger.info("Downloading Secrossphere demo...")
        scs_zip_path = root_path + "scs.zip"
        req = requests.get(scs_url, allow_redirects=True)
        open(scs_zip_path, 'wb').write(req.content)
        robot.api.logger.info("Downloaded Secrossphere demo")
        with zipfile.ZipFile(scs_zip_path, "r", metadata_encoding="cp932") as scs_zip:
            scs_zip.extractall(root_path)
        os.rename(root_path + "セクロスフィア H体験版", scs_path)
        robot.api.logger.info("Extracted Secrossphere demo")

    def patch_secrossphere_demo(self):
        bepinex_zip_path = root_path + "/bepinex32.zip"
        req = requests.get(bepinex32_url, allow_redirects=True)
        open(bepinex_zip_path, 'wb').write(req.content)
        with zipfile.ZipFile(bepinex_zip_path, "r") as bepinex_zip:
            bepinex_zip.extractall(scs_path)
        shutil.copytree(scs_tweaks_path, scs_path, dirs_exist_ok=True)
        shutil.copytree(scs_lovemachine_path, scs_path, dirs_exist_ok=True)
        robot.api.logger.info("Patched Secrossphere demo")

    def start_secrossphere_demo(self):
        self.scs_process = subprocess.Popen([scs_path + "Trial.exe"])
        robot.api.logger.info("Started Secrossphere demo")

    def close_secrossphere_demo(self):
        self.scs_process.terminate()
        robot.api.logger.info("Closed Secrossphere demo")

    def set_secrossphere_resolution(self, width, height, fullscreen):
        key_path = "SOFTWARE\illusion\Secrossphere_Trial"
        winreg.CreateKey(winreg.HKEY_CURRENT_USER, key_path)
        with winreg.OpenKey(winreg.HKEY_CURRENT_USER, key_path, 0, winreg.KEY_WRITE) as key:
            winreg.SetValueEx(key, "Screenmanager Is Fullscreen mode_h3981298716", 0, winreg.REG_DWORD, fullscreen)
            winreg.SetValueEx(key, "Screenmanager Resolution Width_h182942802", 0, winreg.REG_DWORD, width)
            winreg.SetValueEx(key, "Screenmanager Resolution Height_h2627697771", 0, winreg.REG_DWORD, height)
    
    def use_secrossphere_config(self, config_path):
        shutil.copyfile(config_path, scs_path + "UserData/Save/Config")
