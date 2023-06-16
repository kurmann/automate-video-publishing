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

## Wechsel von Python zu .NET
Überlegungen vom 7. Juni 2023 von Python auf das .NET-Framework zu wechseln

### Beweggründe
Bei der Entscheidung für eine Programmiersprache zur Implementierung des Skripts wurden sowohl Python als auch .NET Core in Betracht gezogen. Beide haben ihre Stärken und bieten umfangreiche Funktionen zur Bewältigung der geplanten Aufgaben.

Ein wichtiger Aspekt, der zur Wahl von .NET Core beigetragen hat, ist die Fähigkeit zur Erstellung einer eigenständigen, ausführbaren Datei, die ohne zusätzliche Abhängigkeiten auf verschiedenen Plattformen läuft. Dieser Aspekt der Portabilität und Bereitstellung war entscheidend, da das Ziel darin besteht, das Skript zu entwickeln und auf anderen Maschinen bereitzustellen, ohne zusätzliche Setup-Anforderungen zu haben.

Python, obwohl eine sehr flexible und mächtige Sprache, hat gewisse Herausforderungen in Bezug auf die Erstellung von eigenständigen, ausführbaren Dateien. Während es Werkzeuge wie PyInstaller oder py2exe gibt, die Python-Skripte in ausführbare Dateien umwandeln können, bringen diese ihre eigenen Einschränkungen und Kompatibilitätsprobleme mit sich. Dies kann besonders problematisch sein, wenn das Skript von verschiedenen Personen auf unterschiedlichen Systemen verwendet wird.

Darüber hinaus kann das Management von Paketabhängigkeiten in Python auf verschiedenen Maschinen eine zusätzliche Herausforderung darstellen, insbesondere wenn es darum geht, das Skript auf einem anderen Rechner bereitzustellen.

Mit .NET Core können diese potenziellen Hürden vermieden werden, was es zu einer attraktiven Wahl für das Projekt macht. Zudem bietet .NET Core eine starke Leistung und eine umfangreiche Standardbibliothek, was zusätzlich zu den oben genannten Vorteilen den Ausschlag zugunsten von .NET Core gegeben hat.

### Entscheidungsmatrix

| Kriterium                     | Gewichtung | Python Bewertung[^1] | .NET Bewertung[^2] | Gewichtete Python Bewertung | Gewichtete .NET Bewertung |
|-------------------------------|------------|------------------|----------------|-----------------------------|---------------------------|
| Lernkurve                     | 3          | 1                | 3              | 3                           | 9                         |
| Community                     | 2          | 3                | 2              | 6                           | 4                         |
| Leistung und Effizienz[^3]    | 1          | 1                | 3              | 1                           | 3                         |
| Plattformunabhängigkeit       | 3          | 1                | 3              | 3                           | 9                         |
| Bibliotheken und Frameworks   | 2          | 3                | 3              | 6                           | 6                         |
| **Gesamt**                    | **11**     |                  |                | **19**                      | **31**                    |

[^1]: Python Bewertung: Diese Bewertung basiert auf Ihrer Erfahrung und den Anforderungen Ihrer Projekte. Python ist eine ausgezeichnete Sprache für Anfänger und hat eine sehr aktive Community, was zu einer hohen Bewertung in diesen Kategorien führt. Da Python jedoch eine interpretierte Sprache ist, kann sie bei der Leistung und Effizienz nicht mit .NET mithalten. Auch wenn Python plattformunabhängig ist, unterstützt es nicht die Erstellung von eigenständigen Anwendungen in demselben Ausmaß wie .NET, was zu einer niedrigeren Bewertung in dieser Kategorie führt.

[^2]: .NET Bewertung: Ihre Erfahrung und Komfort mit .NET führt zu einer hohen Bewertung in der Lernkurve. Obwohl die .NET Community groß ist, ist sie im Vergleich zu Python kleiner, daher die niedrigere Bewertung in dieser Kategorie. In Bezug auf Leistung und Effizienz übertrifft .NET Python, da es näher an der Maschine ist und weniger Overhead hat. Schließlich unterstützt .NET die Erstellung von vollständigen, eigenständigen Anwendungen, was zu einer hohen Bewertung in der Kategorie Plattformunabhängigkeit führt.

[^3]: Bei der Bewertung der Leistung und Effizienz ist zu beachten, dass der limitierende Faktor in diesem speziellen Anwendungsfall die Upload-Geschwindigkeit und nicht die Ausführungsgeschwindigkeit der Sprache ist. Dies könnte die tatsächliche Wirkung der Leistung und Effizienz in der realen Anwendung verändern.
