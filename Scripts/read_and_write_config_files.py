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


def create_yaml(buck_bond: list = None, buck_angle: list = None):
    if buck_bond is None and buck_angle is None:
        print("No parameters given.")
        return

    test_data = []

    if buck_bond is not None:
        test_data.append({'xml path': '2_buckyballs.xml', 'pdb path': '2_buckyballs.pdb', 'multipliers': buck_bond,
                          'type of force constant': 'bond', 'type of molecule': 'buckyball'})

    if buck_angle is not None:
        test_data.append({'xml path': '2_buckyballs.xml', 'pdb path': '2_buckyballs.pdb', 'multipliers': buck_angle,
                          'type of force constant': 'angle', 'type of molecule': 'buckyball'})

    write_yaml_to_file(test_data, 'my_yaml')


if __name__ == '__main__':
    # buckyball_bond_multipliers = [1]
    buckyball_angle_multipliers = [0.5, 1, 1.5]

    create_yaml(buck_angle=buckyball_angle_multipliers)
