import os
from PIL import Image

# Target size (exact, will stretch)
WIDTH = 512
HEIGHT = 294

INPUT_DIR = "input_images"
OUTPUT_DIR = "output_images"

os.makedirs(OUTPUT_DIR, exist_ok=True)

def resize_image(input_path, output_path):
    with Image.open(input_path) as img:
        # Convert to RGB to avoid issues with PNG transparency / modes
        img = img.convert("RGB")

        # Resize with stretching (no aspect ratio preservation)
        resized = img.resize((WIDTH, HEIGHT), Image.LANCZOS)

        resized.save(output_path)

def process_directory():
    for filename in os.listdir(INPUT_DIR):
        if filename.lower().endswith((".png", ".jpg", ".jpeg", ".webp", ".bmp", ".gif")):
            input_path = os.path.join(INPUT_DIR, filename)
            output_path = os.path.join(OUTPUT_DIR, filename)

            try:
                resize_image(input_path, output_path)
                print(f"Processed: {filename}")
            except Exception as e:
                print(f"Failed: {filename} -> {e}")

if __name__ == "__main__":
    process_directory()