# Automatisieren von Videoschnitt-Veröffentlichungen

Dieses Projekt ist eine Konsolenanwendung, die auf .NET 7.0 basiert und in C# geschrieben ist. Es dient dazu, den Prozess der Videoschnitt-Veröffentlichung zu automatisieren.

## Zweck

Der Hauptzweck dieser Anwendung besteht darin, den Prozess der Videoschnitt-Veröffentlichung zu automatisieren. Dies wird durch die Implementierung eines Workflows erreicht, der eine Reihe von Schritten in einer bestimmten Reihenfolge ausführt.

## Aufbau

Die Anwendung kann mit Parametern aufgerufen werden:

- `Workflow`: Bestimmt den Automatisierungsablauf, also welche Schritte in welcher Reihenfolge ausgeführt werden.
- `Kontext`: Enthält alle für die Ausführung der Automatisierung notwendigen Daten, z. B. Quelldateien und Zielinformationen für die Metadatenübertragung, AWS-Anmeldedaten usw. Der Kontext kann entweder durch Benutzerspezifische Umgebungsvariablen oder durch eine Konfigurationsdatei (im JSON-Format) definiert werden.

## Leistungsumfang

In der aktuellen Phase implementiert die Anwendung eine Strategie, genannt `MetadataTransferAndArchivingFromMasterfile`, die im Wesentlichen folgendes tut:

- Übertragung der Metadaten von der Apple ProRes Masterdatei in die komprimierte M4V-Datei und Erstellung des M4V-Jahrestags anhand des Iso-Dateinamens der Quelldatei.
- Verschieben der M4V-Datei (die veröffentlichte Datei) in das im Kontext definierte Zielverzeichnis und einsortieren in Unterordner anhand der M4V-Metadaten Albumname und Staffelnummer.
- Archivierung der Masterdatei in AWS und lokale Datei löschen nach Überprüfung durch Checksummen.
- Nach abgeschlossener Übertragung in AWS, den Projektordner des Final Cut Videoschnitts (also die Medienbibliothek) als ZIP zu komprimieren und dann auch in AWS zu übertragen. Das ZIP müsste nach erfolgreicher Übertragung gelöscht werden, einschließlich des Final Cut Projektordners.
- Und bevor dies alles geschieht, müssten die Originalmedien aus dem Final Cut Pro-Projektordners in ein spezielles Verzeichnis "Filmaufnahmen" kopiert werden und anhand der Metadaten in Unterverzeichnisse verpackt werden (also für jeden Tag des Aufnahmezeitpunkts) in Unterordner mit dem Namen des ISO-Datums (YYYY-MM-TT).
- Weitere mögliche Ausbaustufen in Zukunft. Die oben genannte Workflow-Funktionalität soll in Version 1.0 bereitstehen.

## Fortschritt

Der Fortschritt des Projekts kann anhand der folgenden Issues verfolgt werden:

1. [Erstellung des Grundgerüsts der Anwendung](https://github.com/kurmann/automate-video-publishing/issues/11)
2. [Implementierung der Kontextverwaltung](https://github.com/kurmann/automate-video-publishing/issues/12)
3. [Implementierung der Metadatenübertragung](https://github.com/kurmann/automate-video-publishing/issues/13)
4. [Implementierung des Dateiverschiebens](https://github.com/kurmann/automate-video-publishing/issues/14)
5. [Archivierung der Masterdatei in der Cloud](https://github.com/kurmann/automate-video-publishing/issues/15)
6. [Archivierung des Final Cut Projekt-Ordners in der Cloud](https://github.com/kurmann/automate-video-publishing/issues/16)
7. [Implementierung des Kopierens und Verpackens der Originalmedien](https://github.com/kurmann/automate-video-publishing/issues/17)

Bitte beachten Sie, dass die Reihenfolge der Issues der vorgeschlagenen Reihenfolge der Implementierung entspricht.

## Lizenz
Dieses Projekt ist lizenziert unter der MIT-Lizenz. Siehe [LICENSE](LICENSE.txt) für weitere Informationen.

## Archivierter Legacy Code

Das Projekt begann ursprünglich mit Shell- und Python-Skripten. Mit den wachsenden Anforderungen und dem Wunsch nach weniger Konfiguration (all-in-one-Package) wurde jedoch eine Entscheidung getroffen, zu .NET 7.0 zu wechseln. Die genauen Beweggründe für diesen Wechsel sind im entsprechenden Unterkapitel beschrieben.

Die ursprünglichen Skripte boten jedoch einige nützliche Funktionen, wie beispielsweise die automatische Übertragung von Metadaten vom QuickTime Apple ProRes Masterfile zur komprimierten M4V-Datei. Aus diesem Grund wurden diese Skripte archiviert und sind im Verzeichnis [Legacy](https://github.com/kurmann/automate-video-publishing/tree/main/legacy) zu finden. Sie sind dort weiterhin dokumentiert und zugänglich.

Mittelfristig ist geplant, dass die .NET-Anwendung alle Funktionen der ursprünglichen Skripte umfasst und noch viele weitere. Dennoch kann der Zugriff auf den Legacy-Code wertvoll sein, um zu verstehen, wie bestimmte Funktionen ursprünglich implementiert wurden.
