import intifake
import os
import pynput
import requests
import robot
import shutil
import subprocess
import time
import winreg
import zipfile

root_path = "./test/"
bepinex32_url = "https://github.com/BepInEx/BepInEx/releases/download/v5.4.21/BepInEx_x86_5.4.21.0.zip"

scs_url = "https://trial.dlsite.com/professional/VJ016000/VJ015728_trial.zip"
scs_path = root_path + "scs/"
scs_tweaks_path = "../LoveMachine.SCS/tweaks"
scs_lovemachine_path = "../bin/LoveMachine.SCS"

class LoveMachineLibrary:
    
    def __init__(self):
        self._intifake = intifake.Intifake()
        self._mouse = pynput.mouse.Controller()
        self._keyboard = pynput.keyboard.Controller()

    def _timestamp_gaps_should_be_about(self, timestamps, millis):
        tolerance_absolute_ms = 150
        tolerance_relative = 0.2
        gaps = [tup[1] - tup[0] for tup in zip(timestamps[:-1], timestamps[1:])]
        assert all(abs((1000 * gap) - millis) < tolerance_absolute_ms for gap in gaps)
        assert abs(1000 * sum(gaps) / len(gaps) - millis) < millis * tolerance_relative
    
    def start_fake_intiface_server(self):
        self._intifake.start()
        robot.api.logger.info("Started fake Intiface server")
    
    def stop_fake_intiface_server(self):
        self._intifake.stop()
        robot.api.logger.info("Stopped fake Intiface server")

    def press_space_bar(self):
        self._keyboard.tap(pynput.keyboard.Key.space)

    def delete_downloaded_files(self):
        shutil.rmtree(root_path)

    def number_of_linear_commands_should_be_at_least(self, min):
        robot.api.logger.info("Captured linear commands: " + str(self._intifake.linear_commands))
        assert len(self._intifake.linear_commands) >= min
    
    def number_of_vibrate_commands_should_be_at_least(self, min):
        robot.api.logger.info("Captured vibrate commands: " + str(self._intifake.vibrate_commands))
        assert len(self._intifake.vibrate_commands) >= min

    def milliseconds_between_linear_commands_should_be_about(self, millis):
        timestamps = sorted(self._intifake.linear_commands.keys())
        self._timestamp_gaps_should_be_about(timestamps, millis)
        
    def milliseconds_between_vibrate_commands_should_be_about(self, millis):
        timestamps = sorted(self._intifake.vibrate_commands.keys())
        self._timestamp_gaps_should_be_about(timestamps, millis)

    def positions_of_linear_commands_should_alternate(self):
        commands_dict = self._intifake.linear_commands
        timestamps = sorted(commands_dict.keys())
        commands = [commands_dict[t] for t in timestamps]
        positions = [cmd["Vectors"][0]["Position"] for cmd in commands]
        odd_positions = positions[1::2]
        even_positions = positions[::2]
        assert max(odd_positions) - min(odd_positions) < 0.2
        assert max(even_positions) - min(even_positions) < 0.2
        assert abs(max(odd_positions) - min(even_positions)) > 0.5
        assert abs(max(even_positions) - min(odd_positions)) > 0.5

    def no_command_should_have_been_received_in_the_last(self, duration_str):
        seconds = robot.libraries.DateTime.convert_time(duration_str)
        end = time.time() - seconds
        assert all(timestamp < end for timestamp in self._intifake.linear_commands.keys())
        assert all(timestamp < end for timestamp in self._intifake.vibrate_commands.keys())

    def download_secrossphere_demo(self):
        os.mkdir(root_path)
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

    def start_h_scene_in_secrossphere_demo(self):
        self._keyboard.tap("s")
        time.sleep(1)
        self._keyboard.tap(pynput.keyboard.Key.enter)
        time.sleep(5)
        for i in range(14):
            # on github runners fps drops to 4 at this point, so let's click slowly
            self._mouse.press(pynput.mouse.Button.left)
            time.sleep(0.5)
            self._mouse.release(pynput.mouse.Button.left)
            time.sleep(0.5)
        robot.api.logger.info("Started H Scene in Secrossphere demo")
    
    def set_secrossphere_resolution(self, width, height, fullscreen):
        key_path = "SOFTWARE\illusion\Secrossphere_Trial"
        winreg.CreateKey(winreg.HKEY_CURRENT_USER, key_path)
        with winreg.OpenKey(winreg.HKEY_CURRENT_USER, key_path, 0, winreg.KEY_WRITE) as key:
            winreg.SetValueEx(key, "Screenmanager Is Fullscreen mode_h3981298716", 0, winreg.REG_DWORD, fullscreen)
            winreg.SetValueEx(key, "Screenmanager Resolution Width_h182942802", 0, winreg.REG_DWORD, width)
            winreg.SetValueEx(key, "Screenmanager Resolution Height_h2627697771", 0, winreg.REG_DWORD, height)
    
    def use_secrossphere_config(self, config_path):
        shutil.copyfile(config_path, scs_path + "UserData/Save/Config")
