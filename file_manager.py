import sys
import os
import shutil

class FileManager:

    def __init__(self, logger):
        self.logger = logger

    def move_file(self, file_path, album, season):
        # Zielverzeichnis erstellen, wenn es noch nicht existiert
        target_dir = os.path.join("/Volumes/Videoschnitt/Infuse", album, f"Staffel {season}")
        os.makedirs(target_dir, exist_ok=True)

        # Datei in das Zielverzeichnis verschieben
        shutil.move(file_path, target_dir)
        
        return target_dir  # Gebe das Zielverzeichnis zurück, damit wir es ausgeben können