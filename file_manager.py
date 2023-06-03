import sys
import os
import logging
import shutil

def setup_logger(log_file_path):
    logging.basicConfig(filename=log_file_path, level=logging.INFO, 
                        format='%(asctime)s %(levelname)s: %(message)s', 
                        datefmt='%Y-%m-%d %H:%M:%S')
    
def get_file_path():
    # Dateipfad wird als erster Argument beim Skriptaufruf übergeben
    file_path = sys.argv[1]

    # Überprüfung, ob Datei existiert
    if not os.path.isfile(file_path):
        logging.error(f"Datei {file_path} existiert nicht.")
        sys.exit(1)
    
    return file_path

def move_file(file_path, album, season):
    # Zielverzeichnis erstellen, wenn es noch nicht existiert
    target_dir = os.path.join("/Volumes/Videoschnitt/Infuse", album, f"Staffel {season}")
    os.makedirs(target_dir, exist_ok=True)

    # Datei in das Zielverzeichnis verschieben
    shutil.move(file_path, target_dir)
    
    return target_dir  # Gebe das Zielverzeichnis zurück, damit wir es ausgeben können