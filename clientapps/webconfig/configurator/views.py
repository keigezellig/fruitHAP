import requests
from django.core.paginator import Paginator, EmptyPage, PageNotAnInteger
from django.http import Http404
from django.shortcuts import render


def index(request):
    context = dict(current_section='index')
    return render(request, 'index.html', context)


def dashboard(request):
    context = dict(current_section='dashboard')
    return render(request, 'dashboard.html', context)


def sensor_configuration(request):
    req = requests.get("http://localhost/api/configuration/sensors")
    sensor_list = []
    response = req.json()

    for response_item in response:
        item = {'name': response_item['Name'], 'desc': response_item['Description'], 'cat': response_item['Category'],
                'type': response_item['Type']}
        sensor_list.append(item)

    paginator = Paginator(sensor_list, 10)
    page = request.GET.get('page')

    try:
        sensors = paginator.page(page)
    except PageNotAnInteger:
        # If page is not an integer, deliver first page.
        sensors = paginator.page(1)
    except EmptyPage:
        # If page is out of range (e.g. 9999), deliver last page of results.
        sensors = paginator.page(paginator.num_pages)

    context = dict(current_section='configuration', current_page='sensor', number_of_sensors=len(sensor_list),
                   sensor_list=sensors)
    return render(request, 'sensor_configuration.html', context)


def sensor_details(request, sensor_name):
    req = requests.get("http://localhost/api/configuration/sensors/" + sensor_name)
    if req.status_code == 404:
        raise Http404('Sensor does not exist')

    if req.status_code == 200:
        details = req.json()
        context = dict(current_section='configuration', current_page='sensor', number_of_sensors=999, details=details)
        return render(request, 'sensor_details.html', context)
