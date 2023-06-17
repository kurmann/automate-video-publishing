import sys
import os
import logging
from metadata_manager import MetadataReader
from metadata_manager import MetadataWriter
from file_manager import FileManager
from mac_os_interface import NotificationManager
from mac_os_interface import SecureKeyring
from mac_os_interface import KeyNotFoundError

def main():
    """Main function that drives the script."""

    try:
        # File path is passed as the first argument when calling the script
        file_path = sys.argv[1]

        # Log file path corresponds to the path of the file passed
        log_file_path = os.path.splitext(file_path)[0] + ".log"
        logger = setup_logger(log_file_path)

        # Read and log metadata
        metadata_reader = MetadataReader(logger)
        metadata = metadata_reader.get_metadata(file_path)

        # Copy description
        description = metadata_reader.get_description(metadata)
        metadata_writer = MetadataWriter(logger)
        metadata_writer.overwrite_description(file_path, description)

        # Extract tag from the filename and overwrite
        day = metadata_reader.get_date_from_filename(file_path)
        metadata_writer.overwrite_day(file_path, day)
        logger.info(f"Successfully copied description and date to file {file_path}.")

        # Initialize file manager
        file_manager = FileManager(logger)

        # Move file based on album and season number
        album = metadata_reader.get_album(metadata)
        season = metadata_reader.get_tvsn(metadata)
        target_dir = file_manager.move_file(file_path, album, season)  # Save the target directory
        logger.info(f"Successfully moved file to directory {target_dir}.")

        # Verwendung der SecureKeyring-Klasse zum Test
        keyring_instance = SecureKeyring(logger, 'aws-s3-script')
        aws_secret_access_key = keyring_instance.get_key()

        # Benachrichtigung senden Testlauf
        notifier = NotificationManager("Automatisierung Videoveröffentlichung")
        # notifier.send_notification(f"Successfully moved file to directory {target_dir}.")
    
    except KeyNotFoundError as e:
        logger.error(str(e))

    except Exception as e:
        print(str(e))
        sys.exit(1)

def setup_logger(log_file_path):
    """Sets up a logger that writes logs to a file.

    Args:
        log_file_path: Path where the log file should be created.
    Returns:
        The configured logger.
    """
    logger = logging.getLogger()
    logger.setLevel(logging.INFO)

    # File handler
    file_handler = logging.FileHandler(log_file_path)
    file_formatter = logging.Formatter('%(asctime)s %(levelname)s: %(message)s', datefmt='%Y-%m-%d %H:%M:%S')
    file_handler.setFormatter(file_formatter)
    logger.addHandler(file_handler)

    # Console handler
    console_handler = logging.StreamHandler(sys.stdout)
    console_formatter = logging.Formatter('%(asctime)s %(levelname)s: %(message)s', datefmt='%Y-%m-%d %H:%M:%S')
    console_handler.setFormatter(console_formatter)
    logger.addHandler(console_handler)

    return logger

if __name__ == "__main__":
    main()
