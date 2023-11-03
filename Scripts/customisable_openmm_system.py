import openmm.app as app
import openmm as mm
from read_and_write_config_files import read_yaml
import os

# region GLOBAL PARAMETERS

# BUCKYBALLS.
# Bonds.
buckyball_bond_force_constant = 451725
buckyball_r_eq = .1332
# Angles.
buckyball_angle_force_constant = 457.672
buckyball_theta_eq = 2.129301687433082

# ALKANES.
# Bonds.
alkane_bond_force_constant = 270432.69999999995
alkane_r_eq = .15247

# endregion


def read_xml_into_openmm_system(xml_path: str):
    """
    Creates an OpenMM System from xml.
    :param xml_path:
    :return: openmm system.
    """

    with open(xml_path, 'r') as f:
        system_string = f.read()
    system: mm.System
    return mm.XmlSerializer.deserialize(system_string)


class CustomisableOpenMMSystem:
    """
    A customisable OpenMM system.
    """
    openmm_system = None
    pdb_system = None
    angle_atom_ids = None
    openmm_simulation = None
    type_of_molecule = None
    type_of_force_constant = None

    # Simulation parameters.
    bond_k = None
    r_eq = None
    angle_k = None
    theta_eq = None

    # Variable.
    current_multiplier = None

    def __init__(self, xml_path: str, pdb_path: str, type_of_molecule: str, type_of_force_constant: str):
        self.openmm_system = read_xml_into_openmm_system(xml_path)
        self.pdb_system = app.PDBFile(pdb_path)
        self.type_of_molecule = type_of_molecule
        self.type_of_force_constant = type_of_force_constant

    def remove_force(self):
        """
        Removes force from the OpenMM system.
        :return: None
        """

        # Get formatted string that will match the name of the force in the OpenMM system.
        force_string = f'Custom{self.type_of_force_constant.capitalize()}Force'

        # Loop through forces.
        for x, force in enumerate(self.openmm_system.getForces()):

            # Find force to remove.
            if force_string in str(force):

                # Remove force.
                self.openmm_system.removeForce(x)

                # There is only one force that needs to be removed, so return straight away.
                return

    def determine_angle_atom_ids(self):
        """
        Calculate the sets of atom ids in the CustomAngleForce.
        :return atom_ids: A list of OpenMM atom ids required for creating a new CustomAngleForce.
        """

        atom_ids = []

        # Loop through forces.
        for x, force in enumerate(self.openmm_system.getForces()):

            # Retrieve CustomAngleForce from current simulation.
            if 'Angle' in str(force):

                force_to_change = self.openmm_system.getForce(x)

                # Loop through each term.
                for angle_term in range(0, force_to_change.getNumAngles()):

                    # Append atom ids.
                    atom_ids.append([force_to_change.getAngleParameters(angle_term)[0],
                                     force_to_change.getAngleParameters(angle_term)[1],
                                     force_to_change.getAngleParameters(angle_term)[2]])

                # Only need to do this once, so we return straight away.
                return atom_ids

    def create_custom_force(self):
        """
        Handles which force constant is going to be edited.
        :return: None
        """
        if self.type_of_force_constant == 'angle':
            self.create_custom_angle_force()

        if self.type_of_force_constant == 'bond':
            self.create_custom_bond_force()

    def create_custom_bond_force(self):
        """
        Creates a CustomBondForce and adds it to the OpenMM system. Note: currently only works for buckyballs due to
        the hardcoded parameters.
        :return: None
        """

        if self.type_of_molecule == 'buckyball':
            self.bond_k = buckyball_bond_force_constant
            self.r_eq = buckyball_r_eq

        if self.type_of_molecule == 'alkane':
            self.bond_k = alkane_bond_force_constant
            self.r_eq = alkane_r_eq
            # TODO: Currently doesn't work for alkanes
            return None

        if self.bond_k is None or self.r_eq is None:
            raise Exception("Bond force constant or equilibrium bond length not specified, please check that the type "
                            "of molecule is specified in the config file. This value must be 'Alkane' or 'Buckyball'.")

        # Create CustomBondForce.
        custom_force = mm.CustomBondForce(
            'k*0.5*(r-r_eq)*(r-r_eq)*(1.0+cs*(r-r_eq) + (7.0/12.0)*cs*cs*(r-r_eq)*(r-r_eq))')

        # Add global and perBond parameters.
        custom_force.addPerBondParameter('k')  # Bond force constant.
        custom_force.addPerBondParameter('r_eq')  # Equilibrium bond length.
        custom_force.addGlobalParameter('cs', -25.5)  # Scaling constant.
        custom_force.setName('CustomBondForce')  # Name of custom force.

        # TODO: make this work for alkanes

        # Loop through bonds.
        for bond in self.pdb_system.topology.bonds():

            if 0 <= bond.atom1.index < 60:
                # buckyball 1: original.
                custom_force.addBond(bond.atom1.index, bond.atom2.index,
                                     [buckyball_bond_force_constant, buckyball_r_eq])

            else:
                # buckyball 2: altered.
                custom_force.addBond(bond.atom1.index, bond.atom2.index,
                                     [buckyball_bond_force_constant * self.current_multiplier, buckyball_r_eq])

        # Add CustomBondForce.
        self.openmm_system.addForce(custom_force)

    def create_custom_angle_force(self, molecule_id: int):
        """
        Creates a CustomAngleForce and adds it to the OpenMM system. NOTE: Only works for buckyballs due to the
        hardcoded parameters.
        :return: None
        """
        # Create CustomAngleForce.
        custom_force = mm.CustomAngleForce('k *0.5 *dtheta*dtheta*expansion;expansion= 1.0 -0.014*dtor*dtheta+ '
                                           '5.6e-5*dtor^2*dtheta^2-1.0e-6*dtor^3*dtheta^3+2.2e-8*dtor^4*dtheta^4;dtor'
                                           '=57.295779;dtheta = theta- theta_eq')

        # Add global and perBond parameters.
        custom_force.addPerAngleParameter('k')  # Angle force constant.
        custom_force.addPerAngleParameter('theta_eq')  # Equilibrium bond length.
        custom_force.setName('CustomAngleForce')  # Name of custom force.

        # Check that angle ids exist.
        if self.angle_atom_ids is None:
            raise TypeError

        # Set the molecule parameters from the global parameters.
        if self.type_of_molecule == 'buckyball':
            self.angle_k = buckyball_angle_force_constant
            self.theta_eq = buckyball_theta_eq
        else:
            # TODO: doesn't currently work with other types of molecules.
            return

        if self.angle_k is None or self.theta_eq is None:
            raise Exception("Angle force constant or theta_eq not specified, please check that the type of molecule "
                            "is specified in the config file. This value must be 'Alkane' or 'Buckyball'.")

        if molecule_id == 0:
            # Alter the first molecule (A)
            id_min = 0
            id_max = 60
        else:
            # Alter the second molecule (B)
            id_min = 60
            id_max = 120

        # Loop through atoms that require the angle force.
        for ids in self.angle_atom_ids:

            if id_min <= ids[0] < id_max:
                # Alter these force constants.
                custom_force.addAngle(ids[0], ids[1], ids[2], [self.angle_k * self.current_multiplier, self.theta_eq])

            else:
                # Keep original force constants.
                custom_force.addAngle(ids[0], ids[1], ids[2], [self.angle_k, self.theta_eq])

        # Return the OpenMM system with new CustomAngleForce.
        self.openmm_system.addForce(custom_force)

    def create_and_equilibrate_simulation(self):
        """
        Create simulation from the OpenMM system in its current state, runs an energy minimisation and equilibration.
        :return: None
        """
        # Energy minimisation.
        simulation = mm.app.Simulation(self.pdb_system.topology, self.openmm_system,
                                       mm.LangevinIntegrator(300 * mm.unit.kelvin, 1.0 / mm.unit.picosecond,
                                                             1.0 * mm.unit.femtosecond))
        simulation.context.setPositions(self.pdb_system.positions)
        simulation.minimizeEnergy()

        # Set temperature and run simulation for a bit.
        simulation.context.setVelocitiesToTemperature(300 * mm.unit.kelvin)
        simulation.step(1000)

        self.openmm_simulation = simulation

    def write_simulation_to_xml(self, file_name: str):
        """
        Writes the simulation to an xml.
        :return: None
        """

        # Check simulation exists.
        if self.openmm_simulation is None:
            raise TypeError

        file_path = os.path.join('output-xmls', file_name)

        # Write simulation to xml.
        from narupa.openmm import serializer
        with open(file_path, 'w') as outfile:
            outfile.write(serializer.serialize_simulation(self.openmm_simulation))

        print(f'Created xml: {file_name}')

    def change_force_constants(self, multipliers: list):
        """
        Creates a set of xml files with the modified molecular systems.
        :param multipliers: List of multipliers to be applied to the force constants.
        :return: None
        """
        # Loop through multipliers.
        for multiplier in multipliers:

            # Loop through molecule IDs.
            for molecule in range(0, 2):

                # Update multiplier value.
                self.current_multiplier = multiplier

                # Get atom ids for creating CustomAngleForce.
                if self.type_of_force_constant == 'angle':
                    # Get atom ids from current CustomAngleForce.
                    self.angle_atom_ids = self.determine_angle_atom_ids()

                # Remove current force.
                self.remove_force()

                # Create custom force.
                self.create_custom_force(molecule_id=molecule)

                # Create OpenMM simulation.
                self.create_and_equilibrate_simulation()

                # Generate output file path.
                # TODO: make this better
                if molecule == 0:
                    outfile_path = f"{self.type_of_molecule}_{self.type_of_force_constant}_A_{multiplier}.xml"
                else:
                    outfile_path = f"{self.type_of_molecule}_{self.type_of_force_constant}_B_{multiplier}.xml"

                # Write OpenMM Simulation to xml.
                self.write_simulation_to_xml(file_name=outfile_path)


def generate_xml_simulations(yaml_file: str):
    """
    Creates a set of xml files of modified molecular systems and outputs them to the "/output-xmls" directory.
    :param yaml_file: string of the yaml file containing the details of the simulations to be generated.
    :return: None
    """

    # Read data from config file.
    data_from_config_file = read_yaml(yaml_file)

    # Loop through the molecular systems specified in the yaml config file.
    for item in data_from_config_file:

        # Create customisable OpenMM System.
        openmm_system = CustomisableOpenMMSystem(xml_path=item.get('xml path'),
                                                 pdb_path=item.get('pdb path'),
                                                 type_of_molecule=item.get('type of molecule'),
                                                 type_of_force_constant=item.get('type of force constant'))

        # Generate the xml files.
        openmm_system.change_force_constants(multipliers=item.get('multipliers'))


if __name__ == '__main__':

    # ---------- USER TO EDIT ---------- #

    my_yaml_file = 'my_yaml.yaml'

    # ----------- RUN SCRIPT ----------- #
    generate_xml_simulations(yaml_file=my_yaml_file)
