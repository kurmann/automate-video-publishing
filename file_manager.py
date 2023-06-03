import sys
import os
import shutil


class FileManager:
    """Eine Klasse zur Verwaltung von Dateioperationen."""

    def __init__(self, logger):
        """
        Initialisiert das FileManager-Objekt.
        
        Args:
            logger (logging.Logger): Ein Logger-Objekt für die Protokollierung.
        """
        self.logger = logger

    def move_file(self, file_path, album, season):
        """
        Verschiebt eine Datei in ein angegebenes Album und eine Staffel.
        
        Args:
            file_path (str): Der Pfad zur Datei, die verschoben werden soll.
            album (str): Der Name des Albums, in das die Datei verschoben werden soll.
            season (str): Die Staffelnummer, in die die Datei verschoben werden soll.
            
        Returns:
            str: Der Pfad des Zielverzeichnisses, in das die Datei verschoben wurde.
        """
        # Zielverzeichnis erstellen, wenn es noch nicht existiert
        target_dir = os.path.join("/Volumes/Videoschnitt/Infuse", album, f"Staffel {season}")
        os.makedirs(target_dir, exist_ok=True)

        # Datei in das Zielverzeichnis verschieben
        shutil.move(file_path, target_dir)

        # Gebe das Zielverzeichnis zurück, damit wir es ausgeben können
        return target_dir
