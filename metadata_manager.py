import sys
import logging
import subprocess
import os
from datetime import datetime

# Der Pfad zu AtomicParsley
atomic_parsley_path = "/usr/local/bin/AtomicParsley"

# Hilfsfunktion, um Befehle auszuführen und die Ausgabe als Zeichenkette zu erhalten
def run_command(cmd):
    return subprocess.run(cmd, check=True, text=True, capture_output=True).stdout

def setup_logger(log_file_path):
    logging.basicConfig(filename=log_file_path, level=logging.INFO, 
                        format='%(asctime)s %(levelname)s: %(message)s', 
                        datefmt='%Y-%m-%d %H:%M:%S')

def get_metadata(file_path):
    # AtomicParsley Befehl um alle Metadaten auszulesen
    cmd = [atomic_parsley_path, file_path, "-t"]
    metadata = run_command(cmd)

    # Fügt jede Zeile der Metadaten zur Log-Datei hinzu
    logging.info("Folgende MP4-Metadaten wurden aus der Quelldatei ausgelesen.")
    for line in metadata.split('\n'):
        logging.info(line)

    return metadata

def get_description(metadata):
    # Extrahieren der Beschreibung aus den Metadaten
    for line in metadata.split('\n'):
        if line.startswith('Atom "©des" contains: '):
            return line[len('Atom "©des" contains: '):].strip()
    else:
        logging.error("Keine Beschreibung gefunden.")
        sys.exit(1)

def get_tvsn(metadata):
    # Extrahieren der Staffelnummer aus den Metadaten
    for line in metadata.split('\n'):
        if line.startswith('Atom "tvsn" contains: '):
            return line[len('Atom "tvsn" contains: '):].strip()
    else:
        print("Fehler: Keine Staffelnummer gefunden.")
        sys.exit(1)

def get_album(metadata):
    # Extrahieren des Albums aus den Metadaten
    for line in metadata.split('\n'):
        if line.startswith('Atom "©alb" contains: '):
            return line[len('Atom "©alb" contains: '):].strip()
    else:
        print("Fehler: Kein Album gefunden.")
        logging.error("Kein Album gefunden.")
        logging.info("Der Albumname wird benötigt um das Zielverzeichnis zu bestimmen.")
        sys.exit(1)

def overwrite_description(file_path, description):
    # AtomicParsley Befehl um die neue Beschreibung hinzuzufügen
    cmd = [atomic_parsley_path, file_path, "--overWrite", "--description", description]
    run_command(cmd)

def get_day(metadata):
    # Extrahieren des Datums aus den Metadaten
    for line in metadata.split('\n'):
        if line.startswith('Atom "©day" contains: '):
            day = line[len('Atom "©day" contains: '):].strip()
            if 'T' in day:  # Datum enthält bereits eine Zeitangabe
                return day
            else:  # Datum enthält keine Zeitangabe, also fügen wir eine hinzu
                return day + "T12:00:00Z"
    else:
        logging.error("Kein Datum gefunden.")
        sys.exit(1)

def get_date_from_filename(file_path):
    # Extrahieren des Dateinamens aus dem vollständigen Pfad
    filename = os.path.basename(file_path)
    
    # Extrahieren des Datums aus dem Dateinamen
    date_str = filename[:10]
    
    # Überprüfen, ob das Datum im korrekten Format ist
    try:
        datetime.strptime(date_str, "%Y-%m-%d")
    except ValueError:
        logging.error("Datum im Dateinamen {filename} ist nicht im korrekten Format.")
        sys.exit(1)
    
    # Zeit hinzufügen
    return date_str + "T12:00:00Z"

def remove_day(file_path):
    """
    Entfernt das vorhandene "day"-Tag aus der Datei.
    """
    cmd = [atomic_parsley_path, file_path, "--overWrite", "--manualAtomRemove", "moov.udta.meta.ilst.©day"]
    run_command(cmd)

def check_and_modify_date(date_str):
    # Überprüfen, ob die Zeit im Datum enthalten ist, falls nicht, Mittag hinzufügen
    try:
        datetime.strptime(date_str, "%Y-%m-%dT%H:%M:%SZ")  # versuchen, das Datum mit der Zeit zu parsen
    except ValueError:
        date_str += "T12:00:00Z"  # wenn das Parsen fehlschlägt, die Zeit hinzufügen
    return date_str

def overwrite_day(file_path, day):
    """
    Fügt das "day"-Tag zur Datei hinzu.
    """
    remove_day(file_path)  # Zuerst entfernen wir das alte Tag.
    cmd = [atomic_parsley_path, file_path, "--overWrite", "--year", day]
    run_command(cmd)