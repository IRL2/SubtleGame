import json
from narupa.app import NarupaImdClient
from numpy import array as np_array
from knot_pull.config import NUMBER_PRECISION_FUNCTION
from knot_pull.kpclasses import Bead
from knot_pull.bipuller import pull as bipull
from knot_pull.unentangler import unentangle
import time


class RefNarupaknotpullclient:

    resids = None
    atomids = None
    chain = ['A']

    atom_positions = None
    positions_alpha_carbons = None
    number_of_residues = None
    was_knotted = None
    is_currently_knotted = None

    # Knot_pull-specific variables.
    kp_beads = None
    kp_chains = None
    kp_repr_structure = None
    config = {'keep_all': 1, 'quiet': 0, 'outfmt': "xyz", "atom_name": 'CA', 'begin': None, 'end': None,
              'trajectory': False, "outfile": None, 'chains': None, 'default_atom_name': 'CA', 'rna': 0,
              "no_pulling": 0}
    kp_topo = None
    kp_dt_code = None

    def __init__(self, atomids, resids, atom_positions):

        # Get information about the simulation.
        self.atomids = list(atomids)
        self.resids = list(resids)
        self.number_of_residues = len(set(self.resids))
        self.config.update({'chains': self.chain})

        # Check if chain is currently knotted.
        self.check_if_chain_is_knotted(atom_positions=atom_positions, first_check=True)

    def check_if_chain_is_knotted(self, atom_positions, first_check=False):
        """ Uses knot_pull to check if chain is currently knotted. """

        # Update atom positions to those in the current narupa frame.
        self.atom_positions = atom_positions

        # Update positions of alpha carbons.
        self.update_positions_of_alpha_carbons()

        # Run edited knot_pull functions.
        self.run_knot_pull_functions()

        # Check for change in knot state.
        self.check_for_change_in_knot_state(init=first_check)

    def check_for_change_in_knot_state(self, init: bool):
        """ Checks for change in knot state. """

        # Save previous knot state.
        self.was_knotted = self.is_currently_knotted

        # Trefoil Knot.
        if '[3_1](A)' in self.kp_topo:
            self.is_currently_knotted = True

            if (not self.was_knotted) or init:
                print("the trefoil knot!")
            return

        # The Unknot.
        if '[01](A)' in self.kp_topo:
            self.is_currently_knotted = False

            if self.was_knotted or init:
                print("Currently not knotted")
            return

        # A new knot?!
        # TODO: add catch if there is another type of knot.
        self.is_currently_knotted = True
        print("How did you do that? A new knot!")

    def run_knot_pull_functions(self):
        """ Runs edited knot_pull functions to get the knot topology of the protein. """
        self.update_knot_pull_beads()
        self.kp_chains, self.kp_repr_structure = bipull(atoms=self.kp_beads, config=self.config,
                                                        chain_names=self.chain[0],
                                                        get_representative=False, greedy=0)
        self.kp_topo, self.kp_dt_code = unentangle(self.kp_chains)

    def update_knot_pull_beads(self):
        """ Updates the list of Beads used for knot_pull. """

        # Remove previous list of beads.
        self.kp_beads = []

        # Loop through number of alpha carbons.
        for atom_num in range(self.number_of_residues):
            # Get xyz positions of alpha carbon.
            x = self.positions_alpha_carbons[atom_num][0]
            y = self.positions_alpha_carbons[atom_num][1]
            z = self.positions_alpha_carbons[atom_num][2]

            # Create vector of positions.
            current_xyz = np_array(list(map(NUMBER_PRECISION_FUNCTION, [x, y, z])))

            # Create Bead.
            new_bead = Bead(current_xyz, "CA", original_id=self.resids[atom_num])

            # TODO: Original code checked here if atoms are too far apart. Might need to add this in.

            # Append bead to list on atoms.
            self.kp_beads.append(new_bead)

        # Set ids for the beads.
        for i, bead in enumerate(self.kp_beads):
            bead.setId(i)
            if i > 0:
                bead.setNhand(self.kp_beads[i - 1])
            if i < len(self.kp_beads) - 1:
                bead.setChand(self.kp_beads[i + 1])

    def update_positions_of_alpha_carbons(self):
        """ Updates positions of the alpha carbons. """

        # Remove previous positions.
        self.positions_alpha_carbons = []

        # Loop through all atoms.
        for i in range(len(self.atom_positions)):

            # Find alpha carbons.
            if 'CA' in self.atomids[i]:
                # Add positions of each alpha carbon.
                self.positions_alpha_carbons.append(self.atom_positions[i])


def write_frame_to_json(atom_positions: list, resids: list, atomids: list, outfile_name: str):
    """ Write the positions, resids, and atomids of the current frame to json. """

    if not atom_positions:
        raise Exception('No atom positions given.')
    if not resids:
        raise Exception('No residue ids given.')
    if not atomids:
        raise Exception('No atom ids given.')
    atom_positions = list(atom_positions)
    atomids = list(atomids)
    resids = list(resids)
    data = {'positions': atom_positions, 'resids': resids, 'atomids': atomids}
    outfile = outfile_name + '.json'
    with open(outfile, 'w') as file:
        json.dump(data, file)


# USER TO EDIT.
total_time_mins = 5
frequency = 15

if __name__ == '__main__':

    client = NarupaImdClient.autoconnect()
    client.subscribe_to_frames()
    client.update_available_commands()
    knot_pull_client = RefNarupaknotpullclient(atomids=client.current_frame.particle_names,
                                               resids=client.current_frame.residue_ids,
                                               atom_positions=client.current_frame.particle_positions)

    write_frame_to_json(atom_positions=client.current_frame.particle_positions, resids=client.current_frame.residue_ids,
                        atomids=client.current_frame.particle_names, outfile_name='17-ala-data')

    try:
        while True:

            # Check if chain is knotted every n frames.
            for x in range((total_time_mins * 60 * frequency) + 1):
                # Is there a change in knot state?
                knot_pull_client.check_if_chain_is_knotted(atom_positions=client.current_frame.particle_positions)
                time.sleep(1 / frequency)

    # User can quit early with ctrl+c.
    except KeyboardInterrupt:
        print('Stopping knot detection.')
        client.close()