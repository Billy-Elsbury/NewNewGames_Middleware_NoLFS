import ctypes
import time
import keyboard
import math

class PhotogrammetryCapture:
    def __init__(self, camera_speed=float(0), vertical_offset=0):
        self.running = True
        self.camera_speed = camera_speed
        self.vertical_offset = vertical_offset

    def step_back(self, duration=3):
        print("Stepping back...")
        keyboard.press('s')
        time.sleep(duration)
        keyboard.release('s')
        print("Step back complete.")

    def move_mouse(self, dx, dy):
        ctypes.windll.user32.mouse_event(0x0001, int(dx), int(dy), 0, 0)

    def ease_out_cubic(self, t):
        return 1 - math.pow(1 - t, 3)

    def orbit_camera(self, orbit_number):
        print(f"Starting orbit {orbit_number}...")
        orbit_duration = 15
        start_time = time.time()
        vertical_direction = -1 if orbit_number == 2 else 1 if orbit_number == 3 else 0

        while time.time() - start_time < orbit_duration:
            if keyboard.is_pressed('esc'):
                print("Exiting capture...")
                self.running = False
                return

            # Calculate progress for easing
            elapsed_time = time.time() - start_time
            progress = min(elapsed_time / 1, 1) if vertical_direction != 0 else 0
            ease = self.ease_out_cubic(progress)

            # Apply smooth vertical offset during the first second of the orbit
            if vertical_direction != 0 and elapsed_time <= 1:
                move = int(vertical_direction * self.vertical_offset * ease) - int(vertical_direction * self.vertical_offset * self.ease_out_cubic(progress - 0.01))
                self.move_mouse(0, move)

            keyboard.press('d')
            self.move_mouse(-self.camera_speed, 0)
            time.sleep(0.01)

        keyboard.release('d')
        print(f"Completed orbit {orbit_number}.")

    def capture_sequence(self):
        self.step_back(duration=3)
        for i in range(1, 4):
            self.orbit_camera(i)
            if not self.running:
                break

def main():
    capture = PhotogrammetryCapture(camera_speed=3, vertical_offset=500)  # Adjust vertical_offset as needed
    print("Starting capture in 5 seconds...")
    print("Press 'Esc' to exit.")
    time.sleep(5)
    capture.capture_sequence()

if __name__ == "__main__":
    main()