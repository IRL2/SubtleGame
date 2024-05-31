import openmm.app as app
import openmm as mm
from customisable_openmm_system import read_xml_into_openmm_system, write_simulation_to_xml

CUSTOM_FORCE_EQUATION = ("fx + fy + fz; fx=(c12 / rx6^2) - (c6 / rx6); fy=(c12 / ry6^2) - (c6 / ry6); fz=(c12 / rz6^2) "
                         "- (c6 / rz6); rx6=rx^6; ry6=ry^6; rz6=rz^6; rx=min(x+1, mx-x); ry=min(y+1, my-y); rz=min("
                         "z+1, mz-z);")


class OpenMMSystemToAddHardWalls:

    def __init__(self, xml: str, pdb: str, outfile: str):
        self.openmm_system = read_xml_into_openmm_system(xml)
        self.pdb_system = app.PDBFile(pdb)
        self.box_vectors = self.openmm_system.getDefaultPeriodicBoxVectors()
        self.outfile = outfile
        self.openmm_simulation = None

    def add_hard_walls(self):
        # Create custom force
        self.add_custom_external_force()

        # Create OpenMM simulation
        self.create_and_equilibrate_simulation()

        # Write OpenMM Simulation to file
        write_simulation_to_xml(openmm_simulation=self.openmm_simulation, file_name=self.outfile)

    def add_custom_external_force(self):
        custom_force = mm.CustomExternalForce(CUSTOM_FORCE_EQUATION)

        custom_force.addGlobalParameter('mx', self.box_vectors[0].x+1)
        custom_force.addGlobalParameter('my', self.box_vectors[1].y+1)
        custom_force.addGlobalParameter('mz', self.box_vectors[2].z+1)
        custom_force.addGlobalParameter('c6', 0)
        custom_force.addGlobalParameter('c12', 1)

        custom_force.setName('CustomExternalForce')

        # Add all particles
        for particle in range(0, self.openmm_system.getNumParticles()):
            custom_force.addParticle(particle)

        # Add the custom force
        self.openmm_system.addForce(custom_force)

    def create_and_equilibrate_simulation(self):

        simulation = mm.app.Simulation(self.pdb_system.topology, self.openmm_system,
                                       mm.LangevinIntegrator(300 * mm.unit.kelvin, 1.0 / mm.unit.picosecond,
                                                             1.0 * mm.unit.femtosecond))
        simulation.context.setPositions(self.pdb_system.positions)
        self.openmm_simulation = simulation


def generate_xml_simulations(xml: str, pdb: str, outfile: str):
    openmm_system = OpenMMSystemToAddHardWalls(xml=xml, pdb=pdb, outfile=outfile)
    openmm_system.add_hard_walls()


if __name__ == '__main__':
    # ---------- USER TO EDIT ---------- #
    pdb_path = '2_buckyballs.pdb'
    xml_path = '2_buckyballs.xml'
    outfile_path = '2_buckyballs_with_hard_walls.xml'

    # ----------- RUN SCRIPT ----------- #
    generate_xml_simulations(xml=xml_path, pdb=pdb_path, outfile=outfile_path)
