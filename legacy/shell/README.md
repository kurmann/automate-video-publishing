# Shell-Skript für das Hochladen von Dateien zu Amazon S3

Dieses Shell-Skript ermöglicht es, Dateien in einen Amazon S3-Bucket hochzuladen. Im Folgenden wird die Funktion und Struktur des Skripts detailliert erklärt.

## Hinweis zur Skriptentwicklung

> WICHTIG: Das Skript wurde ursprünglich in Python entwickelt und ist während der Entwicklung auf .NET 7.0 umgestiegen. Dieses Shell-Skript ist daher > eine unvollständige Implementierung und wurde noch nicht vollständig getestet. Es wird dennoch zur Verfügung gestellt, falls jemand es nützlich findet und es weiterentwickeln möchte.

## Skriptbeschreibung

Das Skript implementiert eine einfache Funktion, um eine Datei zu Amazon S3 hochzuladen. Es verwendet die `aws` CLI und das `security` Programm, um die AWS-Schlüssel aus dem MacOS-Schlüsselbund abzurufen.

Das Skript besteht aus folgenden Funktionen:

`retrieve_aws_keys()`: Diese Funktion ruft die AWS-Zugriffs- und Geheimschlüssel aus dem Schlüsselbund ab und speichert sie als Umgebungsvariablen.
`upload_file_to_s3()`: Diese Funktion lädt die Datei zu Amazon S3 hoch. Dabei wird der MD5-Hash der Datei generiert und als Metadaten zur S3-Datei hinzugefügt.
`verify_upload()`: Diese Funktion überprüft den erfolgreichen Upload, indem der gespeicherte Hash mit dem Hash der Originaldatei verglichen wird. Bei Übereinstimmung wird die lokale Datei gelöscht und eine Benachrichtigung an den Benutzer gesendet. Bei Nicht-Übereinstimmung wird eine Fehlermeldung generiert und die lokale Datei bleibt unberührt.
Installationsanleitung

Um dieses Skript zu verwenden, müssen Sie sicherstellen, dass die `aws` CLI auf Ihrem System installiert ist und korrekt konfiguriert ist. Sie können die `aws` CLI von der offiziellen AWS-Website herunterladen und installieren.

Stellen Sie außerdem sicher, dass Ihre AWS-Zugriffs- und Geheimschlüssel im MacOS-Schlüsselbund unter den spezifischen Schlüsselnamen gespeichert sind, die in der Funktion `retrieve_aws_keys()` verwendet werden.

## Verwendete Shell-Kommandos

Das Skript verwendet eine Reihe von Shell-Kommandos und -Funktionen, darunter:

`security`: Ein Befehl, der in MacOS verwendet wird, um mit dem Schlüsselbund zu interagieren.
`aws s3 cp`: Ein Befehl der AWS-CLI zum Kopieren von Dateien zu oder von Amazon S3.
`aws s3api head-object`: Ein Befehl der AWS-CLI zum Abrufen von Metadaten eines S3-Objekts.
`osascript`: Ein Befehl, der in MacOS verwendet wird, um AppleScript oder JavaScript für Automation (JXA) Code auszuführen. In diesem Skript wird es verwendet, um Benachrichtigungen an den Benutzer zu senden.
`jq`: Ein Befehl zum Parsen und Manipulieren von JSON-Daten.
Hinweis zur Skriptentwicklung
