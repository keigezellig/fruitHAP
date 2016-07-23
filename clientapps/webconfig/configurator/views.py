import xmlrpc.client

from django.core.paginator import Paginator, EmptyPage, PageNotAnInteger
from django.core.urlresolvers import reverse
from django.http import Http404, HttpResponseRedirect
from django.shortcuts import render
from django.views.generic.base import TemplateView
from configurator.datahelpers import get_sensorlist, get_sensordetails, get_sensorcount


def index(request):
    context = dict(current_section='index')
    return render(request, 'index.html', context)


def dashboard(request):
    context = dict(current_section='dashboard')
    return render(request, 'dashboard.html', context)


def restart(request):
    supervisordServer = xmlrpc.client.Server('http://localhost:9001/RPC2')
    supervisordServer.supervisor.stopProcess('fruithap')
    supervisordServer.supervisor.startProcess('fruithap')

    return HttpResponseRedirect(reverse('sensor_configuration'))


class SensorList(TemplateView):
    template_name = 'sensor_configuration.html'

    def get(self, request, *args, **kwargs):
        sensor_list = get_sensorlist()

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

        number_of_sensors = get_sensorcount()
        if number_of_sensors is None:
            number_of_sensors = 'N/A'

        context = dict(current_section='configuration', current_page='sensor', number_of_sensors=number_of_sensors,
                       sensor_list=sensors)

        return self.render_to_response(context)


class AddSensorView(TemplateView):
    template_name = 'sensor_add.html'

    def get(self, request, *args, **kwargs):
        number_of_sensors = get_sensorcount()

        if number_of_sensors is None:
            number_of_sensors = 'N/A'

        context = dict(current_section='configuration', current_page='sensor', number_of_sensors=number_of_sensors)

        return self.render_to_response(context)


class SensorDetails(TemplateView):
    template_name = 'sensor_details.html'

    def get(self, request, *args, **kwargs):
        sensor_name = self.kwargs['sensor_name']
        details = get_sensordetails(sensor_name)
        if details is None:
            raise Http404('Sensor does not exist')

        number_of_sensors = get_sensorcount()
        if number_of_sensors is None:
            number_of_sensors = 'N/A'

        context = dict(current_section='configuration', current_page='sensor', number_of_sensors=number_of_sensors,
                       details=details)
        return self.render_to_response(context)
