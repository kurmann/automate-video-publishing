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
