import math
import time
import pyautogui
import keyboard


class PhotogrammetryCapture:
    def __init__(self):
        self.running = True
        self.center_x, self.center_y = pyautogui.size()[0] // 2, pyautogui.size()[1] // 2
        self.orbit_radius = 200
        self.current_angle = 0

    def step_back(self):
        keyboard.press('s')
        time.sleep(2)  # Adjust duration as needed
        keyboard.release('s')

    def orbit_camera(self):
        while self.running:
            if keyboard.is_pressed('a'):
                self.current_angle -= 1
            elif keyboard.is_pressed('d'):
                self.current_angle += 1

            x = self.center_x + int(self.orbit_radius * math.cos(math.radians(self.current_angle)))
            y = self.center_y + int(self.orbit_radius * math.sin(math.radians(self.current_angle)))
            pyautogui.moveTo(x, y)

            time.sleep(0.01)

    def capture_sequence(self):
        self.step_back()
        self.orbit_camera()


def main():
    capture = PhotogrammetryCapture()
    print("Starting capture in 5 seconds...")
    print("Press 'Esc' to exit.")
    time.sleep(5)
    capture.capture_sequence()


if __name__ == "__main__":
    main()