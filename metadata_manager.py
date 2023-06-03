import sys
import subprocess
import os
from datetime import datetime

class MetadataManager:

    # Der Pfad zu AtomicParsley
    _atomic_parsley_path = "/usr/local/bin/AtomicParsley"

    def __init__(self, logger):
        self.logger = logger

    # Private Hilfsfunktion, um Befehle auszuführen und die Ausgabe als Zeichenkette zu erhalten
    def _run_command(self, cmd):
        try:
            return subprocess.run(cmd, check=True, text=True, capture_output=True).stdout
        except subprocess.CalledProcessError as e:
            self.logger.error(f"Beim Ausführen des Befehls {cmd} ist ein Fehler aufgetreten: {e}")
            sys.exit(1)

    def log_metadata(self, metadata):
        # Logging der Metadaten
        self.logger.info("Folgende MP4-Metadaten wurden aus der Quelldatei ausgelesen.")
        for line in metadata.split('\n'):
            self.logger.info(line)
    
class MetadataReader(MetadataManager):

    def __init__(self, logger):
        super().__init__(logger)

    def get_metadata(self, file_path):
        # AtomicParsley Befehl um alle Metadaten auszulesen
        cmd = [self._atomic_parsley_path, file_path, "-t"]
        metadata = self._run_command(cmd)

        return metadata

    def get_description(self, metadata):
        # Extrahieren der Beschreibung aus den Metadaten
        for line in metadata.split('\n'):
            if line.startswith('Atom "©des" contains: '):
                return line[len('Atom "©des" contains: '):].strip()
        else:
            raise ValueError("Keine Beschreibung gefunden.")

    def get_tvsn(self, metadata):
        # Extrahieren der Staffelnummer aus den Metadaten
        for line in metadata.split('\n'):
            if line.startswith('Atom "tvsn" contains: '):
                return line[len('Atom "tvsn" contains: '):].strip()
        else:
            print("Fehler: Keine Staffelnummer gefunden.")
            sys.exit(1)

    def get_album(self, metadata):
        # Extrahieren des Albums aus den Metadaten
        for line in metadata.split('\n'):
            if line.startswith('Atom "©alb" contains: '):
                return line[len('Atom "©alb" contains: '):].strip()
        else:
            print("Fehler: Kein Album gefunden.")
            self.logger.error("Kein Album gefunden.")
            self.logger.info("Der Albumname wird benötigt um das Zielverzeichnis zu bestimmen.")
            sys.exit(1)

    def get_day(self, metadata):
        # Extrahieren des Datums aus den Metadaten
        for line in metadata.split('\n'):
            if line.startswith('Atom "©day" contains: '):
                day = line[len('Atom "©day" contains: '):].strip()
                if 'T' in day:  # Datum enthält bereits eine Zeitangabe
                    return day
                else:  # Datum enthält keine Zeitangabe, also fügen wir eine hinzu
                    return day + "T12:00:00Z"
        else:
            self.logger.error("Kein Datum gefunden.")
            sys.exit(1)

    def get_date_from_filename(self, file_path):
        # Extrahieren des Dateinamens aus dem vollständigen Pfad
        filename = os.path.basename(file_path)
        
        # Extrahieren des Datums aus dem Dateinamen
        date_str = filename[:10]
        
        # Überprüfen, ob das Datum im korrekten Format ist
        try:
            datetime.strptime(date_str, "%Y-%m-%d")
        except ValueError:
            self.logger.error("Datum im Dateinamen {filename} ist nicht im korrekten Format.")
            sys.exit(1)
        
        # Zeit hinzufügen
        return date_str + "T12:00:00Z"

class MetadataWriter(MetadataManager):

    def __init__(self, logger):
        super().__init__(logger)

    def overwrite_description(self, file_path, description):
        # AtomicParsley Befehl um die neue Beschreibung hinzuzufügen
        cmd = [self._atomic_parsley_path, file_path, "--overWrite", "--description", description]
        self._run_command(cmd)
    
    def remove_day(self, file_path):
        """
        Entfernt das vorhandene "day"-Tag aus der Datei.
        """
        cmd = [self._atomic_parsley_path, file_path, "--overWrite", "--manualAtomRemove", "moov.udta.meta.ilst.©day"]
        self._run_command(cmd)

    def check_and_modify_date(self, date_str):
        # Überprüfen, ob die Zeit im Datum enthalten ist, falls nicht, Mittag hinzufügen
        try:
            datetime.strptime(date_str, "%Y-%m-%dT%H:%M:%SZ")  # versuchen, das Datum mit der Zeit zu parsen
        except ValueError:
            date_str += "T12:00:00Z"  # wenn das Parsen fehlschlägt, die Zeit hinzufügen
        return date_str

    def overwrite_day(self, file_path, day):
        """
        Fügt das "day"-Tag zur Datei hinzu.
        """
        self.remove_day(file_path)  # Zuerst entfernen wir das alte Tag.
        cmd = [self._atomic_parsley_path, file_path, "--overWrite", "--year", day]
        self._run_command(cmd)
