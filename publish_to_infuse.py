import sys
import os
import logging
from metadata_manager import MetadataReader
from metadata_manager import MetadataWriter
from file_manager import FileManager

def main():

    try:

        # Dateipfad wird als erster Argument beim Skriptaufruf übergeben
        file_path = sys.argv[1]

        # Logdateipfad entspricht dem Pfad der übergebenen Datei
        log_file_path = os.path.splitext(file_path)[0] + ".log"
        logger = setup_logger(log_file_path)

        # Metadaten auslesen und loggen
        metadata_reader = MetadataReader(logger)
        metadata = metadata_reader.get_metadata(file_path)
        metadata_reader.log_metadata(metadata)

        # Beschreibung kopieren
        description = metadata_reader.get_description(metadata)
        metadata_writer = MetadataWriter(logger)
        metadata_writer.overwrite_description(file_path, description)

        # Tag aus dem Dateinamen extrahieren und überschreiben
        day = metadata_reader.get_date_from_filename(file_path)
        metadata_writer.overwrite_day(file_path, day)
        logger.info(f"Beschreibung und Datum erfolgreich kopiert in Datei {file_path}.")

        # Dateimanager initalisieren
        file_manager = FileManager(logger)

        # Datei verschieben anhand Album und Staffelnummer
        album = metadata_reader.get_album(metadata)
        season = metadata_reader.get_tvsn(metadata)
        target_dir = file_manager.move_file(file_path, album, season)  # Speichere das Zielverzeichnis
        logger.info(f"Datei erfolgreich in das Verzeichnis {target_dir} verschoben.")

    except Exception as e:
        sys.stderr.write(str(e))
        sys.exit(1)

# Eine Funktion, um den Logger zu erstellen
def setup_logger(log_file_path):
    logger = logging.getLogger()
    logger.setLevel(logging.INFO)
    handler = logging.FileHandler(log_file_path)
    formatter = logging.Formatter('%(asctime)s %(levelname)s: %(message)s', datefmt='%Y-%m-%d %H:%M:%S')
    handler.setFormatter(formatter)
    logger.addHandler(handler)
    return logger

if __name__ == "__main__":
    main()
