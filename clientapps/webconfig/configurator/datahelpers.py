import requests


def get_sensorlist():
    sensor_list = []
    req = requests.get("http://localhost/api/configuration/sensors")
    if req.status_code == 200:
        response = req.json()
        for response_item in response:
            item = {'name': response_item['Name'], 'desc': response_item['Description'],
                    'cat': response_item['Category'],
                    'type': response_item['Type']}
            sensor_list.append(item)

    return sensor_list


def get_sensordetails(sensor_name):
    req = requests.get("http://localhost/api/configuration/sensors/" + sensor_name)
    if req.status_code == 200:
        return req.json()
    else:
        return None


def get_sensorcount():
    count_req = requests.get("http://localhost/api/configuration/sensors/count")
    if count_req.status_code == 200:
        return count_req.json()
    else:
        return None


def get_sensortypes():
    types_req = requests.get("http://localhost/api/configuration/sensors/types")
    if types_req.status_code == 200:
        return types_req.json()
    else:
        return None


def get_sensorparameters(type_name):
    types_req = requests.get("http://localhost/api/configuration/sensors/types/" + type_name)
    if types_req.status_code == 200:
        return types_req.json()
    else:
        return None


def delete_sensor(sensor_name):
    delete_req = requests.delete("http://localhost/api/configuration/sensors/delete/" + sensor_name)

    return delete_req.status_code == 200
