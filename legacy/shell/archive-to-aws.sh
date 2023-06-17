# The first argument is the path of the file
# Use ´$1´as file name if file is passed by Apple Automator
FILE=""
BUCKET=""
TARGET_DIR=""
FILENAME=$(basename "$FILE")
KEY="$TARGET_DIR/$FILENAME"
LOGFILE="${FILE%.*}.log"

function retrieve_aws_keys() {
    # Retrieve the AWS access key and secret key from the keychain
    AWS_ACCESS_KEY_ID=$(security find-generic-password -s archive-to-aws-access-key-keychain-keyname -w)
    AWS_SECRET_ACCESS_KEY=$(security find-generic-password -s archive-to-aws-secret-key-keychain-keyname -w)

    # Check if the access key and secret key were retrieved successfully
    if [[ -z "$AWS_ACCESS_KEY_ID" || -z "$AWS_SECRET_ACCESS_KEY" ]]; then
        echo "Fehler: Konnte AWS-Zugriffs- oder Geheimschlüssel nicht aus dem Schlüsselbund auslesen."
        osascript -e "display notification \"Fehler: Konnte AWS-Zugriffs- oder Geheimschlüssel nicht aus dem Schlüsselbund auslesen. Das Skript wurde beendet.\" with title \"Fehler beim Auslesen der AWS-Schlüssel\""
        exit 1
    else
        echo "Erfolgreich AWS-Zugriffs- und Geheimschlüssel aus dem Schlüsselbund ausgelesen."
    fi

    # Export the keys as environment variables
    export AWS_ACCESS_KEY_ID
    export AWS_SECRET_ACCESS_KEY
}

function upload_file_to_s3() {
    FILE=$1
    BUCKET=$2
    KEY=$3
    # Generate the MD5 hash of the file
    HASH=$(md5 -q "$FILE")
    echo Local hash value: $HASH

    # Upload the file to S3, adding the hash as metadata
    echo Beginne mit dem Upload...
    aws s3 cp "${FILE}" "s3://${BUCKET}/${KEY}" --metadata "md5hash=${HASH}"
    echo "Uploaded $FILE to s3://${BUCKET}/${KEY}"
}

function verify_upload() {
    BUCKET=$1
    KEY=$2
    LOCAL_HASH=$3
    LOGFILE=$4

    # Get the object metadata
    echo "Getting metadata for: s3://${BUCKET}/${KEY}"
    METADATA=$(aws s3api head-object --bucket $BUCKET --key "$KEY")

    # Extract the stored hash from the metadata
    STORED_HASH=$(echo $METADATA | jq -r .Metadata.md5hash)
    echo Stored hash value of uploaded AWS file: $STORED_HASH

    # Compare the stored hash with the hash of the file
    if [ "$LOCAL_HASH" == "$STORED_HASH" ]; then
        # The file is intact. Delete the local file.
        echo Hash values of AWS and local file are equal. Delete the local file to complete the archiving process.
        rm "$FILE"
        # Send a notification to the user
        osascript -e 'display notification "Die Datei wurde erfolgreich hochgeladen und lokal gelöscht." with title "Archivierung erfolgreich"'
        echo "Die Datei wurde erfolgreich hochgeladen und lokal gelöscht."
    else
        # The file may have been corrupted. Don't delete the file.
        # Send a notification to the user
        osascript -e "display notification \"Ein Fehler ist aufgetreten. Weitere Details finden Sie in der Logdatei: $LOGFILE\" with title \"Archivierung fehlgeschlagen\""
        echo "Die Datei wurde nicht korrekt hochgeladen."
    fi
}

# Output to both console and log file
{
    retrieve_aws_keys
    upload_file_to_s3 "$FILE" "$BUCKET" "$KEY"
    verify_upload "$BUCKET" "$KEY" "$HASH" "$LOGFILE"
} 2>&1 | tee "$LOGFILE"