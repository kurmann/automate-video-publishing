import sys
import publish_to_infuse

def write_mp4_metadata_test():
    test_input = "/Volumes/Videoschnitt/Ablauf/03 Veröffentlichen/2023-05-29 Margeriten-Insel-Familienfilme.m4v"

    try:
        sys.argv = ["", test_input]  # "" corresponds to the script name (sys.argv[0])
        publish_to_infuse.main()
    except Exception as e:
        print(f"Write mp4 metadate failed with exception: {str(e)}")

if __name__ == "__main__":
    write_mp4_metadata_test()