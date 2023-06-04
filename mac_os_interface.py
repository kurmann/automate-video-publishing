from pync import Notifier

class NotificationManager:
    """
    Eine Klasse, die die Verwaltung von Desktop-Benachrichtigungen in MacOS abstrahiert.
    Nutzt die Bibliothek `pync`.
    """

    def __init__(self, title):
        """
        Initialisiert das NotificationManager-Objekt.

        Parameter:
        title (str): Der Standardtitel für alle gesendeten Benachrichtigungen.
        """
        self.title = title

    def send_notification(self, message):
        """
        Sendet eine Benachrichtigung mit dem voreingestellten Titel und der angegebenen Nachricht.

        Parameter:
        message (str): Die Nachricht, die in der Benachrichtigung angezeigt wird.
        """
        Notifier.notify(message, title=self.title)
