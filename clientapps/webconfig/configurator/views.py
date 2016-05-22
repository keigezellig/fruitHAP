from django.shortcuts import render
import requests
import json


def index(request):
    context = dict(currentPage='index')
    return render(request, 'index.html', context)


def configuration(request):
    context = dict(currentPage='configuration', number_of_sensors=9)
    return render(request, 'configuration.html', context)


def sensor_configuration(request):
    req = requests.get("http://sarajevo:8888/api/configuration/sensors")
    sensor_list = []
    response = json.loads(req.text)

    for response_item in response:

        valueurl = response_item['SupportedOperations'].get('GetValue')
        if valueurl is None:
            value = 'N/A'
        else:
            value = json.loads(requests.get(valueurl).text)

        item = {'name': response_item['Name'], 'desc': response_item['Description'], 'cat': response_item['Category'],
                'type': response_item['Type'], 'value': value}
        sensor_list.append(item)

    context = dict(currentPage='configuration', number_of_sensors=len(sensor_list), sensor_list=sensor_list)
    return render(request, 'sensor_configuration.html', context)
