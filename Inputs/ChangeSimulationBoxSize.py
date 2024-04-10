import xml.etree.ElementTree as ET
import os


def update_xml_files(dir_path):
    # Iterate over all XML files in the given directory
    for filename in os.listdir(dir_path):
        if filename.endswith(".xml"):
            full_path = os.path.join(dir_path, filename)

            # Parse the XML file
            tree = ET.parse(full_path)
            root = tree.getroot()

            # Navigate to PeriodicBoxVectors and update attributes specifically
            vectors = root.find('.//PeriodicBoxVectors')
            if vectors is not None:
                for vector in vectors:
                    if vector.tag == 'A':
                        vector.set('x', '5')  # Only change x for <A>
                    elif vector.tag == 'B':
                        vector.set('y', '5')  # Only change y for <B>
                    elif vector.tag == 'C':
                        vector.set('z', '5')  # Only change z for <C>

            # Write the updated XML back to the file
            tree.write(full_path)
            print(f"Updated {filename}")


if __name__ == '__main__':
    directory_path = 'ANGLE'
    update_xml_files(dir_path=directory_path)
