# Archivierter Legacy Code

Das ursprüngliche Projekt begann mit einer Mischung aus Shell- und Python-Skripten, die spezifische Automatisierungsaufgaben durchführten. Mit zunehmender Komplexität und dem Wunsch nach einem einfacheren, all-in-one Paket, wurde die Entscheidung getroffen, auf .NET 7.0 umzusteigen.

Trotz dieser Änderung können die ursprünglichen Skripte immer noch nützlich sein, insbesondere für spezifische Funktionen wie die automatische Übertragung von Metadaten vom Apple QuickTime ProRes-Masterfile zur komprimierten M4V-Datei. Aus diesem Grund sind sie archiviert und dokumentiert.

Mittelfristig wird die .NET-Anwendung voraussichtlich alle Funktionen der alten Skripte übernehmen und noch mehr bieten.

Die alten Skripte können in den folgenden Verzeichnissen gefunden werden:

- Python-Skripte: [Link zum Verzeichnis](python)
- Shell-Skript: [Link zum Verzeichnis](shell)

Bitte beachten Sie, dass diese Skripte als "Legacy" betrachtet werden und möglicherweise nicht mehr aktiv unterstützt oder gewartet werden.

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
