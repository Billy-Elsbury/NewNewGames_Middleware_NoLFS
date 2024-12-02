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
        self.orbit_steps = 0

    def step_back(self, duration=3):
        """Moves the character back for the given duration."""
        keyboard.press('s')
        time.sleep(duration)
        keyboard.release('s')

    def orbit_camera(self):
        """Circles around the object three times, changing the vertical angle."""
        vertical_angles = [0, -10, 10]  # Look straight, slightly down, slightly up

        for angle in vertical_angles:
            for _ in range(360):  # One full circle per angle
                if not self.running:
                    break

                # Check if the Escape key is pressed to exit
                if keyboard.is_pressed('esc'):
                    print("Exiting capture...")
                    self.running = False
                    break

                # Adjust the angle for horizontal orbit
                if keyboard.is_pressed('a'):
                    self.current_angle -= 1
                elif keyboard.is_pressed('d'):
                    self.current_angle += 1

                # Calculate the new position based on the orbit radius and angle
                x = self.center_x + int(self.orbit_radius * math.cos(math.radians(self.current_angle)))
                y = self.center_y + int(self.orbit_radius * math.sin(math.radians(self.current_angle)))

                # Move the cursor to the new position
                pyautogui.moveTo(x, y)

                # Adjust the camera angle vertically
                pyautogui.moveTo(x, y + int(angle))  # Simulate looking up/down

                time.sleep(0.01)

    def capture_sequence(self):
        self.step_back(duration=3)  # Move back for 3 seconds
        self.orbit_camera()  # Orbit around the object


def main():
    capture = PhotogrammetryCapture()
    print("Starting capture in 5 seconds...")
    print("Press 'Esc' to exit.")
    time.sleep(5)
    capture.capture_sequence()


if __name__ == "__main__":
    main()
