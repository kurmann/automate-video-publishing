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

import keyring
import sys

class KeyNotFoundError(Exception):
    pass

class SecureKeyring:
    def __init__(self, logger, key_name):
        self.service_id = "MP4 Veröffentlichungs-Automation"
        self.key_name = key_name
        self.logger = logger

    def get_key(self):
        key = keyring.get_password(self.service_id, self.key_name)

        if key is None:
            self.logger.error(f"Key '{self.key_name}' not found. Please add this key manually to your MacOS Keyring.")
            raise KeyNotFoundError(f"Key '{self.key_name}' not found.")
        
        return key

