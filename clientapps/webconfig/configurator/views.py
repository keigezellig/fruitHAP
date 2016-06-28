import requests
from django.core.paginator import Paginator, EmptyPage, PageNotAnInteger
from django.core.urlresolvers import reverse, reverse_lazy
from django.http import Http404
from django.shortcuts import render
from django.views.generic import FormView
from django.views.generic.base import TemplateView

from configurator.my_forms import SensorForm


def index(request):
    context = dict(current_section='index')
    return render(request, 'index.html', context)


def dashboard(request):
    context = dict(current_section='dashboard')
    return render(request, 'dashboard.html', context)


class SensorList(TemplateView):
    template_name = 'sensor_configuration.html'

    def get(self, request, *args, **kwargs):
        req = requests.get("http://localhost/api/configuration/sensors")
        sensor_list = []
        response = req.json()

        for response_item in response:
            item = {'name': response_item['Name'], 'desc': response_item['Description'],
                    'cat': response_item['Category'],
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

        return self.render_to_response(context)


class SensorDetails(TemplateView):
    template_name = 'sensor_details.html'

    def get(self, request, *args, **kwargs):
        sensor_name = self.kwargs['sensor_name']
        req = requests.get("http://localhost/api/configuration/sensors/" + sensor_name)
        if req.status_code == 404:
            raise Http404('Sensor does not exist')

        if req.status_code == 200:
            details = req.json()
            number_of_sensors = 'N/A'
            countReq = requests.get("http://localhost/api/configuration/sensors/getCount")
            if countReq.status_code == 200:
                number_of_sensors = countReq.json()

            context = dict(current_section='configuration', current_page='sensor', number_of_sensors=number_of_sensors,
                           details=details)
            return self.render_to_response(context)


class SensorFormView(FormView):
    template_name = 'sensor_form.html'
    form_class = SensorForm
    success_url = reverse_lazy('sensor_configuration')

    def form_valid(self, form):
        form.update_config()
        return super(SensorFormView, self).form_valid(form)
