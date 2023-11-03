import yaml


def read_yaml(file_path):
    '''
    Reads ya aml config file and returns the data as a dictionary.
    :param file_path:
    :return:
    '''
    with open(file_path, "r") as f:
        return yaml.safe_load(f)


def write_yaml_to_file(config_data: list, filename: str):
    """
    Writes a yaml config file from a dictionary.
    :param config_data:
    :param filename:
    :return:
    """
    with open(f'{filename}.yaml', 'w', ) as f:
        yaml.dump(config_data, f, sort_keys=False)

    print('Written to file successfully')


def create_yaml(alk_bond, buck_bond, buck_angle):

    test_data = [
        # TODO: add this once alkane functionality is added.
        # {'xml path': '2_C20.xml', 'pdb path': '2_C20.pdb', 'multipliers': alk_bond,
        #  'type of force constant': 'bond', 'type of molecule': 'alkane'},
        {'xml path': '2_buckyballs.xml', 'pdb path': '2_buckyballs.pdb', 'multipliers': buck_bond,
         'type of force constant': 'bond', 'type of molecule': 'buckyball'},
        {'xml path': '2_buckyballs.xml', 'pdb path': '2_buckyballs.pdb', 'multipliers': buck_angle,
         'type of force constant': 'angle', 'type of molecule': 'buckyball'}
    ]

    write_yaml_to_file(test_data, 'my_yaml')


if __name__ == '__main__':
    #[0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9]
    alkane_bond_multipliers = [0.9, 1.05]
    buckyball_bond_multipliers = [1]
    buckyball_angle_multipliers = [1]

    create_yaml(alk_bond=alkane_bond_multipliers, buck_bond=buckyball_bond_multipliers,
                buck_angle=buckyball_angle_multipliers)
