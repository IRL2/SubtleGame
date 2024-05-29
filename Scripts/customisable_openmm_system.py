import openmm.app as app
import openmm as mm
from read_and_write_config_files import read_yaml
import os
from nanover.openmm import serializer

# region Original buckyball simulation parameters

buckyball_bond_force_constant = 451725
buckyball_r_eq = .1332

buckyball_angle_force_constant = 457.672
buckyball_theta_eq = 2.129301687433082


# endregion


class CustomisableOpenMMSystem:
    """
    A customisable OpenMM system for modifying force constants of buckyball simulations.
    """
    openmm_system = None
    pdb_system = None
    angle_atom_ids = None
    openmm_simulation = None
    type_of_force_constant_to_alter = None
    angle_force_string = 'angle'
    bond_force_string = 'bond'

    # Simulation parameters
    bond_k = None
    r_eq = None
    angle_k = None
    theta_eq = None
    current_multiplier = None
    run_simulation = None

    def __init__(self, xml_path: str, pdb_path: str, type_of_force_constant: str, run_simulation=True):
        self.openmm_system = read_xml_into_openmm_system(xml_path)
        self.pdb_system = app.PDBFile(pdb_path)
        self.type_of_force_constant_to_alter = type_of_force_constant
        self.run_simulation = run_simulation

    def remove_force(self):
        """
        Removes the CustomAngleForce or CustomBondForce from the OpenMM system.
        :return: None
        """

        # Get type of force to remove
        force_type = f'Custom{self.type_of_force_constant_to_alter.capitalize()}Force'

        # Loop through forces in the system
        for x, force in enumerate(self.openmm_system.getForces()):

            # Find force to remove
            if force_type in str(force):
                self.openmm_system.removeForce(x)

                # Only one force needs to be removed, so return straight away
                return

    def determine_angle_atom_ids(self):
        """
        Calculate the sets of atom ids in the CustomAngleForce.
        :return atom_ids: A list of OpenMM atom ids required for creating a new CustomAngleForce.
        """

        atom_ids = []

        # Loop through forces in the system
        for x, force in enumerate(self.openmm_system.getForces()):

            # Retrieve CustomAngleForce
            if self.angle_force_string in str(force).lower():

                force_to_change = self.openmm_system.getForce(x)

                # Loop through each term
                for angle_term in range(0, force_to_change.getNumAngles()):
                    # Append atom ids
                    atom_ids.append([force_to_change.getAngleParameters(angle_term)[0],
                                     force_to_change.getAngleParameters(angle_term)[1],
                                     force_to_change.getAngleParameters(angle_term)[2]])

                # Only need to do this once, so return straight away
                return atom_ids

    def add_custom_force(self, molecule_id_to_alter: int):
        """
        Retrieves the custom force and adds it to the system.
        :return: None
        """

        if molecule_id_to_alter == 0:
            # Alter the first molecule, A
            id_min = 0
            id_max = 60
        else:
            # Alter the second molecule, B
            id_min = 60
            id_max = 120

        if self.type_of_force_constant_to_alter == self.angle_force_string:
            custom_force = self.get_custom_angle_force(id_min=id_min, id_max=id_max)

        elif self.type_of_force_constant_to_alter == self.bond_force_string:
            custom_force = self.get_custom_bond_force(id_min=id_min, id_max=id_max)

        else:
            raise ValueError("Type of force constant was not recognised.")

        self.openmm_system.addForce(custom_force)

    def get_custom_angle_force(self, id_min: int, id_max: int):
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
            raise TypeError("No angle atom ids found.")

        # Set the molecule parameters from the global parameters.
        self.angle_k = buckyball_angle_force_constant
        self.theta_eq = buckyball_theta_eq

        # Loop through atoms that require the angle force.
        for ids in self.angle_atom_ids:

            # Is the first atom in the set is in the molecule to be altered?
            if id_min <= ids[0] < id_max:
                # Alter force constants
                custom_force.addAngle(ids[0], ids[1], ids[2], [self.angle_k * self.current_multiplier, self.theta_eq])

            else:
                # Keep original force constants
                custom_force.addAngle(ids[0], ids[1], ids[2], [self.angle_k, self.theta_eq])

        return custom_force

    def get_custom_bond_force(self, id_min: int, id_max: int):
        """
        Creates a CustomBondForce and adds it to the OpenMM system. Note: currently only works for buckyballs due to
        the hardcoded parameters.
        :return: None
        """

        self.bond_k = buckyball_bond_force_constant
        self.r_eq = buckyball_r_eq

        custom_force = mm.CustomBondForce(
            'k*0.5*(r-r_eq)*(r-r_eq)*(1.0+cs*(r-r_eq) + (7.0/12.0)*cs*cs*(r-r_eq)*(r-r_eq))')

        custom_force.addPerBondParameter('k')  # Bond force constant
        custom_force.addPerBondParameter('r_eq')  # Equilibrium bond length
        custom_force.addGlobalParameter('cs', -25.5)  # Scaling constant
        custom_force.setName('CustomBondForce')  # Name of custom force

        # Loop through atoms that require the angle force.
        for bond in self.pdb_system.topology.bonds():

            # Is the first atom in the set is in the molecule to be altered?
            if id_min <= bond.atom1.index < id_max:
                # Alter the force constant
                custom_force.addBond(bond.atom1.index, bond.atom2.index,
                                     [buckyball_bond_force_constant * self.current_multiplier, buckyball_r_eq])

            else:
                # Keep original force constants
                custom_force.addBond(bond.atom1.index, bond.atom2.index,
                                     [buckyball_bond_force_constant, buckyball_r_eq])

        return custom_force

    def create_and_equilibrate_simulation(self):
        """
        Creates simulation from the OpenMM system and runs an energy minimisation then equilibration simulation.
        :return: None
        """
        # Energy minimisation
        simulation = mm.app.Simulation(self.pdb_system.topology, self.openmm_system,
                                       mm.LangevinIntegrator(300 * mm.unit.kelvin, 1.0 / mm.unit.picosecond,
                                                             1.0 * mm.unit.femtosecond))
        simulation.context.setPositions(self.pdb_system.positions)
        simulation.minimizeEnergy()

        # Set temperature and run simulation for a bit
        if self.run_simulation:
            simulation.context.setVelocitiesToTemperature(300 * mm.unit.kelvin)
            simulation.step(1000)

        self.openmm_simulation = simulation

    def change_force_constants(self, multipliers: list):
        """
        Writes a set of xml files for the modified molecular systems.
        :param multipliers: List of multipliers to be applied to the force constants.
        :return: None
        """

        # Loop through multipliers to be applied to the force constants
        for multiplier in multipliers:

            self.current_multiplier = multiplier

            # Loop through molecule IDs
            # NOTE: This is in order to create two simulations, one with A altered and the other with B altered
            for molecule_to_alter in range(0, 2):

                # Get atom ids for creating CustomAngleForce
                if self.type_of_force_constant_to_alter == self.angle_force_string:
                    self.angle_atom_ids = self.determine_angle_atom_ids()

                # Remove current force
                self.remove_force()

                # Create custom force
                self.add_custom_force(molecule_id_to_alter=molecule_to_alter)

                # Create and equilibrate OpenMM simulation
                self.create_and_equilibrate_simulation()

                # Generate output file path
                if molecule_to_alter == 0:
                    outfile_path = f"buckyball_{self.type_of_force_constant_to_alter}_A_{multiplier}.xml"
                else:
                    outfile_path = f"buckyball_{self.type_of_force_constant_to_alter}_B_{multiplier}.xml"

                # Write OpenMM Simulation to file
                write_simulation_to_xml(openmm_simulation=self.openmm_simulation, file_name=outfile_path)


def read_xml_into_openmm_system(xml_path: str):
    """
    Creates an OpenMM System from an .xml file.
    :param xml_path:
    :return: openmm system.
    """

    with open(xml_path, 'r') as f:
        system_string = f.read()
    system: mm.System
    return mm.XmlSerializer.deserialize(system_string)


def write_simulation_to_xml(openmm_simulation, file_name: str):
    """
    Writes the simulation to an .xml file.
    :return: None
    """

    # Check simulation exists
    if openmm_simulation is None:
        raise TypeError("No simulation found.")

    # Generate an .xml file
    file_path = os.path.join('output-xmls', file_name)
    with open(file_path, 'w') as outfile:
        outfile.write(serializer.serialize_simulation(openmm_simulation))
    print(f'Created xml: {file_name}')


def generate_xml_simulations(yaml_file: str, run_simulation: bool):
    """
    Creates a set of xml files of modified molecular systems and outputs them to the "/output-xmls" directory.
    :param yaml_file: String of the yaml file containing the details of the simulations to be generated.
    :param run_simulation: Specify whether to run a simulation after energy minimisation. Set to False if you want to
    keep the exact coordinates specified in the input xml.
    :return: None
    """

    # Read data from .yaml config file
    data_from_config_file = read_yaml(yaml_file)

    # Loop through the molecular systems
    for item in data_from_config_file:

        # Create customisable OpenMM System
        openmm_system = CustomisableOpenMMSystem(xml_path=item.get('xml path'),
                                                 pdb_path=item.get('pdb path'),
                                                 type_of_force_constant=item.get('type of force constant'),
                                                 run_simulation=run_simulation)

        # Generate the xml files
        openmm_system.change_force_constants(multipliers=item.get('multipliers'))


if __name__ == '__main__':
    # ---------- USER TO EDIT ---------- #
    my_yaml_file = 'my_yaml.yaml'

    # ----------- RUN SCRIPT ----------- #
    generate_xml_simulations(yaml_file=my_yaml_file, run_simulation=False)
