# The first argument is the path of the file
FILE="" # Example: /Volumes/Videos/Archive/Movie.m4v
BUCKET="" # The AWS bucket name
TARGET_DIR="" # Example: Archive/2023
LOGFILE="" # Example: /Volumes/Videos/Archive-to-AWS.log

# Extract the filename from the path
FILENAME=$(basename "$FILE")

# Use the filename as part of the key
KEY="$TARGET_DIR/$FILENAME"

{
    # Retrieve the AWS access key and secret key from the keychain
    AWS_ACCESS_KEY_ID=$(security find-generic-password -s archive-to-aws-access-key-keychain-keyname -w)
    AWS_SECRET_ACCESS_KEY=$(security find-generic-password -s archive-to-aws-secret-key-keychain-keyname -w)

    # Export the keys as environment variables
    export AWS_ACCESS_KEY_ID
    export AWS_SECRET_ACCESS_KEY

    # Generate the MD5 hash of the file
    HASH=$(md5 -q "$FILE")

    # Upload the file to S3, adding the hash as metadata
    aws s3 cp "${FILE}" "s3://${BUCKET}/${KEY}" --metadata "md5hash=${HASH}"

    # Get the object metadata
    echo "Getting metadata for: s3://${BUCKET}/${KEY}"
    METADATA=$(aws s3api head-object --bucket $BUCKET --key "$KEY")

    # Extract the stored hash from the metadata
    STORED_HASH=$(echo $METADATA | jq -r .Metadata.md5hash)

    # Compare the stored hash with the hash of the file
    if [ "$HASH" == "$STORED_HASH" ]; then
        # The file is intact. Delete the local file.
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

} 2>&1 | tee "$LOGFILE"