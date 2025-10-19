import glob
import os
import pynput
import requests
import robot
import shutil
import subprocess
import time


class GameLibrary:
    ROBOT_LIBRARY_SCOPE = "SUITE"

    def __init__(self):
        self._mouse = pynput.mouse.Controller()
        self._keyboard = pynput.keyboard.Controller()

    def launch_game(self, path):
        self.game_process = subprocess.Popen(path)
        robot.api.logger.info("Game started")

    def close_game(self):
        self.game_process.terminate()
        robot.api.logger.info("Game closed")

    def copy_content(self, src, dst):
        for file in glob.glob(src):
            shutil.copytree(file, dst, dirs_exist_ok=True)
    
    def download_zip(self, url, dst):
        zip_path = f"{dst}.zip"
        if os.path.exists(zip_path):
            robot.api.logger.info(f"File {zip_path} already present, not downloading")
        else:
            req = requests.get(url, allow_redirects=True)
            open(zip_path, 'wb').write(req.content)
            robot.api.logger.info(f"Downloaded {url} to {zip_path}")
        shutil.unpack_archive(zip_path, dst)

    def press_key(self, key):
        self._keyboard.tap(key if len(key) == 1 else pynput.keyboard.Key[key])
        time.sleep(1)

    def left_click(self):
        self._mouse.click(pynput.mouse.Button.left)
        time.sleep(1)
