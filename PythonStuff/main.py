import math
import time
from pynput.mouse import Button, Controller as MouseController
from pynput.keyboard import Key, Controller as KeyboardController, Listener


class PhotogrammetryCapture:
    def __init__(self):
        self.mouse = MouseController()
        self.keyboard = KeyboardController()
        self.center_x, self.center_y = 960, 540
        self.running = True
        self.start_time = time.time()

    def move_camera(self, dx, dy):
        self.mouse.move(dx, dy)

    def orbit_camera(self, radius, angle_step, loops, elevation):
        for _ in range(loops):
            for angle in range(0, 360, angle_step):
                if not self.running:
                    return
                x = self.center_x + int(radius * math.cos(math.radians(angle)))
                y = self.center_y + int(radius * math.sin(math.radians(angle)))
                self.mouse.position = (x, y + elevation)
                time.sleep(0.05)

    def step_back(self):
        self.keyboard.press(Key.shift)
        self.keyboard.press('s')
        time.sleep(2)
        self.keyboard.release('s')
        self.keyboard.release(Key.shift)

    def on_press(self, key):
        if key == Key.esc:
            self.running = False
            return False

    def capture_sequence(self):
        with Listener(on_press=self.on_press) as listener:
            self.step_back()

            while self.running:
                self.orbit_camera(200, 5, 3, 0)
                self.orbit_camera(200, 5, 3, 50)
                self.orbit_camera(200, 5, 3, -50)

                if not self.running:
                    break

            listener.join()

        elapsed_time = time.time() - self.start_time
        print(f"Total capture time: {elapsed_time:.2f} seconds")


def main():
    capture = PhotogrammetryCapture()
    print("Switching to application in 5 seconds...")
    print("Press 'Esc' to exit the script at any time.")
    time.sleep(5)
    capture.capture_sequence()


if __name__ == "__main__":
    main()