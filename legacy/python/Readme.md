# Legacy Python-Code zur Automatisierung des Videoschnitts

In diesem Ordner finden Sie den Python-Code, der zur Automatisierung des Videoschnittprozesses verwendet wurde. Die Skripte und Klassen sind in verschiedene Dateien aufgeteilt, die jeweils verschiedene Aufgaben erfüllen. In den folgenden Abschnitten werden diese einzelnen Skripte und ihre Funktionen erläutert.

## Datei: file_manager.py

In dieser Datei ist eine Klasse `FileManager` definiert, die verschiedene Dateioperationen verwaltet.

### Klasse: FileManager
Die `FileManager` Klasse ist eine Hilfsklasse zur Verwaltung von Dateioperationen. Sie bietet eine Methode zur Verschiebung einer Datei in ein angegebenes Album und eine Staffel auf einem bestimmten Laufwerk. Der Pfad des Zielverzeichnisses wird dabei dynamisch auf Grundlage der übergebenen Album- und Staffelinformationen erstellt.

Die Klasse hat eine Methode namens `move_file`, die eine Datei an einen bestimmten Ort verschiebt. Sie erwartet den Pfad der zu verschiebenden Datei sowie den Namen des Albums und die Staffelnummer, in die die Datei verschoben werden soll. Diese Methode erstellt das Zielverzeichnis, wenn es noch nicht existiert, verschiebt die Datei in das Zielverzeichnis und gibt den Pfad des Zielverzeichnisses zurück.

## Datei: test_publish_to_infuse.py

Diese Datei beinhaltet ein Testskript zur Überprüfung der Funktionalität der `publish_to_infuse` Funktion. Sie stellt sicher, dass diese Funktion korrekt arbeitet, indem sie einen Testlauf mit einer gegebenen Eingabe durchführt.

Die Hauptfunktion dieses Skriptes, `write_mp4_metadata_test`, setzt einen Testpfad als Eingabe für die `publish_to_infuse` Funktion und versucht, sie auszuführen. Wenn dabei eine Ausnahme auftritt, wird diese gefangen und der Fehlertext ausgegeben.

## Datei: mac_os_interface.py

Diese Datei enthält mehrere Klassen zur Interaktion mit verschiedenen Aspekten des MacOS Betriebssystems.

### Klasse: NotificationManager
Die Klasse `NotificationManager` verwaltet die Erzeugung von Desktop-Benachrichtigungen auf einem MacOS-System. Sie verwendet dazu die pync Bibliothek. Sie hat eine Methode `send_notification`, die eine Benachrichtigung mit dem im Konstruktor angegebenen Titel und der übergebenen Nachricht sendet.

### Klasse: SecureKeyring
Die `SecureKeyring` Klasse interagiert mit dem MacOS Schlüsselbund (Keychain) zur sicheren Speicherung von Schlüsseln. Sie hat eine Methode `get_key`, die einen Schlüssel aus dem Schlüsselbund abruft. Wenn der Schlüssel nicht gefunden wird, wirft diese Methode eine `KeyNotFoundError` Ausnahme.

## Datei: metadata_manager.py

Diese Datei enthält mehrere Klassen zur Verwaltung von Metadaten.

### Klasse: MetadataManager
Die `MetadataManager` Klasse dient zur Ausführung von `AtomicParsley` Befehlen und zum Protokollieren von Metadaten. Sie hat eine Methode `_run_command`, die einen übergebenen Befehl ausführt und die Ausgabe als Zeichenkette zurückgibt. Es gibt auch eine Methode `log_metadata`, die die Metadaten einer bestimmten Datei in das Protokoll schreibt.

### Klasse: MetadataReader
Die `MetadataReader` Klasse erbt von der `MetadataManager` Klasse und liest Metadaten aus MP4-Dateien. Sie hat eine Methode `read_metadata`, die Metadaten aus einer angegebenen Datei liest und ein Dictionary mit den Metadaten zurückgibt.

### Klasse: MetadataWriter
Die `MetadataWriter` Klasse erbt auch von der `MetadataManager` Klasse und schreibt Metadaten in MP4-Dateien. Sie hat eine Methode `write_metadata` die Metadaten in eine angegebene Datei schreibt.

## Datei: main.py

Dieses Skript ist der Hauptausführungspunkt für die Automatisierung des Videoschnittprozesses. Es erstellt Instanzen der oben genannten Klassen, liest Metadaten aus einer MP4-Datei, ändert sie, verschiebt die Datei anhand der Metadaten und sendet schließlich eine Benachrichtigung über den Erfolg der Operation.
