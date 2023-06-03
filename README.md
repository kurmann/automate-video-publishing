# MP4 Veröffentlichungs-Automation

## Überblick
Dieses Python-Projekt zielt darauf ab, den Prozess der Veröffentlichung von MP4-Dateien zu automatisieren, insbesondere indem es die Metadaten aus den ursprünglichen MOV-Masterdateien aus Final Cut Pro in die komprimierten MP4-Dateien überträgt. Es kann hilfreich sein für Anwendungsfälle, die eine MP4-Veröffentlichung erfordern.

## Features
- Auslesen der Metadaten aus der Originaldatei
- Kopieren der Beschreibung und des Datums aus der Metadaten in die MP4-Datei
- Verschieben der MP4-Datei in ein Zielverzeichnis basierend auf den Metadaten

## Voraussetzungen
- Python (3.6 oder höher)
- AtomicParsley

## Installation
1. Klonen Sie das Repository: `git clone https://github.com/kurmann/automate-video-publishing`
2. Wechseln Sie in das Verzeichnis des geklonten Repositorys: `cd mp4-publishing-automation`
3. Installieren Sie die erforderlichen Python-Pakete: `pip install -r requirements.txt`

## Verwendung
Führen Sie das Hauptskript `publishing_to_infuse.py` mit dem Pfad zur zu verarbeitenden MP4-Datei als Argument aus. Zum Beispiel: `python publishing_to_infuse.py /path/to/your/video.mp4`

## Lizenz
Dieses Projekt ist lizenziert unter der MIT-Lizenz. Siehe [LICENSE](LICENSE) für weitere Informationen.
