import intifake
import os
import pynput
import requests
import robot
import shutil
import subprocess
import time
import zipfile
import winreg

root_path = "./test/"
bepinex32_url = "https://github.com/BepInEx/BepInEx/releases/download/v5.4.21/BepInEx_x86_5.4.21.0.zip"

scs_url = "https://trial.dlsite.com/professional/VJ016000/VJ015728_trial.zip"
scs_path = root_path + "scs/"
scs_tweaks_path = "../LoveMachine.SCS/tweaks"
scs_lovemachine_path = "../bin/LoveMachine.SCS"

class LoveMachineLibrary(object):
    
    def _timestamp_gaps_should_be_about(self, timestamps, millis):
        gaps = list(map(lambda tup: tup[1] - tup[0], zip(timestamps[:-1], timestamps[1:])))
        for gap in gaps:
            assert abs((gap * 1000) - millis) < millis * 0.5

    def start_fake_intiface_server(self):
        intifake.start()
        robot.api.logger.info("Started fake Intiface server")
    
    def stop_fake_intiface_server(self):
        intifake.stop()
        robot.api.logger.info("Stopped fake Intiface server")

    def delete_downloaded_files(self):
        shutil.rmtree(root_path)

    def number_of_linear_commands_should_be_at_least(self, min):
        robot.api.logger.info("Captured linear commands: " + str(intifake.linear_commands))
        assert len(intifake.linear_commands) >= min

    def milliseconds_between_linear_commands_should_be_about(self, millis):
        timestamps = list(intifake.linear_commands.keys())
        timestamps.sort()
        self._timestamp_gaps_should_be_about(timestamps, millis)
        
    # TODO
    def positions_of_linear_commands_should_alternate(self):
        pass
    
    def milliseconds_between_vibrate_commands_should_be_about(self, millis):
        timestamps = list(intifake.vibrate_commands.keys())
        timestamps.sort()
        self._timestamp_gaps_should_be_about(timestamps, millis)

    def download_secrossphere_demo(self):
        os.mkdir(root_path)
        robot.api.logger.info("Downloading Secrossphere demo...")
        scs_zip_path = root_path + "scs.zip"
        req = requests.get(scs_url, allow_redirects = True)
        open(scs_zip_path, 'wb').write(req.content)
        robot.api.logger.info("Downloaded Secrossphere demo")
        with zipfile.ZipFile(scs_zip_path, "r", metadata_encoding = "cp932") as scs_zip:
            scs_zip.extractall(root_path)
        os.rename(root_path + "セクロスフィア H体験版", scs_path)
        robot.api.logger.info("Extracted Secrossphere demo")

    def patch_secrossphere_demo(self):
        bepinex_zip_path = root_path + "/bepinex32.zip"
        req = requests.get(bepinex32_url, allow_redirects = True)
        open(bepinex_zip_path, 'wb').write(req.content)
        with zipfile.ZipFile(bepinex_zip_path, "r") as bepinex_zip:
            bepinex_zip.extractall(scs_path)
        shutil.copytree(scs_tweaks_path, scs_path, dirs_exist_ok = True)
        shutil.copytree(scs_lovemachine_path, scs_path, dirs_exist_ok = True)
        robot.api.logger.info("Patched Secrossphere demo")

    def start_secrossphere_demo(self):
        self.scs_process = subprocess.Popen([scs_path + "Trial.exe"])
        robot.api.logger.info("Started Secrossphere demo")

    def close_secrossphere_demo(self):
        self.scs_process.terminate()
        robot.api.logger.info("Closed Secrossphere demo")

    def start_h_scene_in_secrossphere_demo(self):
        mouse = pynput.mouse.Controller()
        keyboard = pynput.keyboard.Controller()
        keyboard.type("s")
        time.sleep(1)
        keyboard.press(pynput.keyboard.Key.enter)
        time.sleep(1)
        keyboard.release(pynput.keyboard.Key.enter)
        time.sleep(5)
        for i in range(14):
            mouse.press(pynput.mouse.Button.left)
            time.sleep(0.5)
            mouse.release(pynput.mouse.Button.left)
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
