import os
import requests
import robot
import shutil
import subprocess
import winreg


root_path = "./bin/"
bepinex32_url = "https://github.com/BepInEx/BepInEx/releases/download/v5.4.22/BepInEx_x86_5.4.22.0.zip"
scs_url = "https://trial.dlsite.com/professional/VJ016000/VJ015728_trial.zip"
scs_path = root_path + "scs/"
scs_tweaks_path = "../LoveMachine.SCS/tweaks"
scs_lovemachine_path = "../bin/LoveMachine.SCS"


class SecrossphereLibrary:
    ROBOT_LIBRARY_SCOPE = "SUITE"

    def download_secrossphere_demo(self):
        scs_zip_path = root_path + "scs.zip"
        if os.path.exists(scs_zip_path):
            robot.api.logger.info("Secrossphere demo already present, not downloading.")
        else:
            req = requests.get(scs_url, allow_redirects=True)
            open(scs_zip_path, 'wb').write(req.content)
            robot.api.logger.info("Downloaded Secrossphere demo")
        shutil.unpack_archive(scs_zip_path, root_path)
        folder_name = "セクロスフィア H体験版".encode('cp932').decode('cp437')
        os.rename(root_path + folder_name, scs_path)
        robot.api.logger.info("Extracted Secrossphere demo")

    def patch_secrossphere_demo(self):
        bepinex_zip_path = root_path + "bepinex32.zip"
        req = requests.get(bepinex32_url, allow_redirects=True)
        open(bepinex_zip_path, 'wb').write(req.content)
        shutil.unpack_archive(bepinex_zip_path, scs_path)
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
