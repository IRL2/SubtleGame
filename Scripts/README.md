# Instructions

1. Open this directory in your Python IDE.
2. Set the python interpreter to "subtle-game" as per the instructions in the README for this Git repo.
3. Open the "read_and_write_config_files.py" script and specify the details of the xml simulations you want in the two lists at the bottom of the script. Run this script to create a yaml config file.
4. Run the "customisable_openmm_system.py" without changing anything. This will use the yaml config file you just created to generate the simulations.
5. Your xml files will be output in the "/output-xmls" directory.

Each xml file contains two buckyball molecules (labelled A and B). Two simulation xml files will be generated per force constant specified in the yaml: (1) molecule A is modified and B is the original buckyball and (2) A is the original buckyball and molecule B is modified.
