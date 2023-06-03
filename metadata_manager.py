import sys
import subprocess
import os
from datetime import datetime

import subprocess
import sys

class MetadataManager:
    """
    Die Klasse MetadataManager dient zum Ausführen von AtomicParsley-Befehlen und zum Protokollieren von Metadaten.
    """
    # Der Pfad zu AtomicParsley
    _atomic_parsley_path = "/usr/local/bin/AtomicParsley"

    def __init__(self, logger):
        """
        Initialisiert den MetadataManager.

        :param logger: Logger-Instanz für die Protokollierung.
        """
        self.logger = logger

    def _run_command(self, cmd):
        """
        Führt den übergebenen Befehl aus und gibt die Ausgabe als Zeichenkette zurück.

        :param cmd: Der auszuführende Befehl.
        :return: Die Ausgabe des Befehls.
        """
        try:
            return subprocess.run(cmd, check=True, text=True, capture_output=True).stdout
        except subprocess.CalledProcessError as e:
            self.logger.error(f"Beim Ausführen des Befehls {cmd} ist ein Fehler aufgetreten: {e}")
            sys.exit(1)

    def log_metadata(self, metadata):
        """
        Protokolliert die übergebenen Metadaten.

        :param metadata: Die zu protokollierenden Metadaten.
        """
        self.logger.info("Folgende MP4-Metadaten wurden aus der Quelldatei ausgelesen.")
        for line in metadata.split('\n'):
            self.logger.info(line)

class MetadataNotFoundException(Exception):
    """
    Eine benutzerdefinierte Exception, die ausgelöst wird, wenn ein erwartetes Metadatum nicht gefunden wird.
    """
    pass

class InvalidDateFormatException(Exception):
    """
    Eine benutzerdefinierte Exception, die ausgelöst wird, wenn das Datum im Dateinamen nicht im erwarteten Format ist.
    """
    pass

class MetadataReader(MetadataManager):
    """
    Eine Unterklasse von MetadataManager, die spezifisch für das Lesen von Metadaten ist.
    """
    def __init__(self, logger):
        super().__init__(logger)

    def get_metadata(self, file_path):
        """
        Ruft alle Metadaten von der angegebenen Datei ab.
        """
        cmd = [self._atomic_parsley_path, file_path, "-t"]
        return self._run_command(cmd)

    def _get_metadata_value(self, metadata, atom):
        """
        Eine private Methode zum Abrufen eines spezifischen Metadatenwerts.
        """
        for line in metadata.split('\n'):
            if line.startswith(f'Atom "{atom}" contains: '):
                return line[len(f'Atom "{atom}" contains: '):].strip()
        raise MetadataNotFoundException(f"Kein Wert für Atom '{atom}' gefunden.")

    def get_description(self, metadata):
        """
        Ruft die Beschreibung aus den Metadaten ab.
        """
        return self._get_metadata_value(metadata, '©des')

    def get_tvsn(self, metadata):
        """
        Ruft die TV-Seriennummer aus den Metadaten ab.
        """
        return self._get_metadata_value(metadata, 'tvsn')

    def get_album(self, metadata):
        """
        Ruft das Album aus den Metadaten ab.
        """
        return self._get_metadata_value(metadata, '©alb')

    def get_day(self, metadata):
        """
        Ruft das Datum aus den Metadaten ab und fügt ggf. eine Zeitangabe hinzu.
        """
        day = self._get_metadata_value(metadata, '©day')
        return day if 'T' in day else day + "T12:00:00Z"

    def get_date_from_filename(self, file_path):
        """
        Ruft das Datum aus dem Dateinamen der angegebenen Datei ab.
        """
        # Extrahieren des Dateinamens aus dem vollständigen Pfad
        filename = os.path.basename(file_path)
        
        # Extrahieren des Datums aus dem Dateinamen
        date_str = filename[:10]
        
        # Überprüfen, ob das Datum im korrekten Format ist
        try:
            datetime.strptime(date_str, "%Y-%m-%d")
        except ValueError:
            raise InvalidDateFormatException(f"Datum im Dateinamen {filename} ist nicht im korrekten Format.")
        
        # Zeit hinzufügen
        return date_str + "T12:00:00Z"

class MetadataWriter:

    _atomic_parsley_path = "/usr/local/bin/AtomicParsley"

    def __init__(self, logger):
        """Initializes the MetadataWriter with the logger to be used.

        Args:
            logger: A logger to log information and errors.
        """
        self.logger = logger

    def _run_command(self, cmd):
        """Executes a command and returns the output as a string.

        Args:
            cmd: List of command strings.
        Returns:
            The stdout of the command execution as a string.
        """
        try:
            return subprocess.run(cmd, check=True, text=True, capture_output=True).stdout
        except subprocess.CalledProcessError as e:
            self.logger.error(f"An error occurred while executing the command {cmd}: {e}")
            sys.exit(1)

    def overwrite_description(self, file_path, description):
        """Writes a new description to a file's metadata.

        Args:
            file_path: Path of the file.
            description: Description to write to the file's metadata.
        """
        cmd = [self._atomic_parsley_path, file_path, "--overWrite", "--description", description]
        self._run_command(cmd)

    def remove_day(self, file_path):
        """Removes the "day" tag from a file's metadata.

        Args:
            file_path: Path of the file.
        """
        cmd = [self._atomic_parsley_path, file_path, "--overWrite", "--manualAtomRemove", "moov.udta.meta.ilst.©day"]
        self._run_command(cmd)

    def check_and_modify_date(self, date_str):
        """Verifies a date string format and adds the time if not present.

        Args:
            date_str: Date string to check and modify.
        Returns:
            Modified date string with added time if it was not present.
        """
        try:
            datetime.strptime(date_str, "%Y-%m-%dT%H:%M:%SZ")
        except ValueError:
            date_str += "T12:00:00Z"
        return date_str

    def overwrite_day(self, file_path, day):
        """Writes a new day to a file's metadata.

        Args:
            file_path: Path of the file.
            day: Day to write to the file's metadata.
        """
        self.remove_day(file_path)
        cmd = [self._atomic_parsley_path, file_path, "--overWrite", "--year", day]
        self._run_command(cmd)
