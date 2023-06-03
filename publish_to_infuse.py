import sys
import os
import logging
import metadata_manager
import file_manager

def setup_logger(log_file_path):
    logging.basicConfig(filename=log_file_path, level=logging.INFO, 
                        format='%(asctime)s %(levelname)s: %(message)s', 
                        datefmt='%Y-%m-%d %H:%M:%S')

def main():

    try:
        # Dateipfad wird als erster Argument beim Skriptaufruf übergeben
        file_path = file_manager.get_file_path()

        # Logging einrichten
        log_file_path = os.path.splitext(file_path)[0] + ".log"
        setup_logger(log_file_path)

        # Metadaten auslesen
        metadata = metadata_manager.get_metadata(file_path)

        # Beschreibung kopieren
        description = metadata_manager.get_description(metadata)
        metadata_manager.overwrite_description(file_path, description)

        # Tag aus dem Dateinamen extrahieren und überschreiben
        day = metadata_manager.get_date_from_filename(file_path)
        metadata_manager.overwrite_day(file_path, day)

        logging.info(f"Beschreibung und Datum erfolgreich kopiert in Datei {file_path}.")

        # Datei verschieben anhand Album und Staffelnummer
        album = metadata_manager.get_album(metadata)
        season = metadata_manager.get_tvsn(metadata)
        target_dir = file_manager.move_file(file_path, album, season)  # Speichere das Zielverzeichnis
        logging.info(f"Datei erfolgreich in das Verzeichnis {target_dir} verschoben.")

    except Exception as e:
        sys.stderr.write(str(e))
        sys.exit(1)

if __name__ == "__main__":
    main()
