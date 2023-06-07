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
    '''
    Writes a yaml config file from a dictionary.
    :param config_data:
    :param filename:
    :return:
    '''
    with open(f'{filename}.yaml', 'w', ) as f:
        yaml.dump(config_data, f, sort_keys=False)

    print('Written to file successfully')


if __name__ == '__main__':

    test_data = [
        {'xml path': '2_C20.xml', 'pdb path': '2_C20.pdb', 'multipliers': [0.9, 1.05],
         'type of force constant': 'Bond', 'type of molecule': 'Alkane'},
        {'xml path': '2_buckyballs.xml', 'pdb path': '2_buckyballs.pdb', 'multipliers': [0.9, 1.05],
         'type of force constant': 'Bond', 'type of molecule': 'Buckyball'},
        {'xml path': '2_buckyballs.xml', 'pdb path': '2_buckyballs.pdb', 'multipliers': [0.9, 1.05],
         'type of force constant': 'Angle', 'type of molecule': 'Buckyball'}
    ]

    write_yaml_to_file(test_data, 'my_yaml')
