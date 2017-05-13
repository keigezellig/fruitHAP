from django.conf.urls import url
from django.contrib import admin
from django.core.urlresolvers import reverse

from configurator import views
from configurator.views import AddSensorView
from configurator.views import DeleteSensorView
from configurator.views import SensorDetails
from configurator.views import SensorList
from configurator.views import SiteView

urlpatterns = [
    url(r'^$', views.index, name='index'),
    url(r'^restart/$', views.restart, name='restart'),
    url(r'^site/$', SiteView.as_view(), name='site'),
    url(r'^configuration/sensors/(?P<sensor_name>[^/]+)', SensorDetails.as_view(), name='sensor_details'),
    url(r'^configuration/deletesensor/$', DeleteSensorView.as_view(), name='sensor_delete'),
    url(r'^configuration/sensors/$', SensorList.as_view(), name='sensor_configuration'),
    url(r'^configuration/addsensor/$', AddSensorView.as_view(), name='sensor_add'),
    url(r'^dashboard/$', views.dashboard, name='dashboard'),
    url(r'^admin/', admin.site.urls),
]


def javascript_settings():
    js_conf = {
        'site_url': reverse('site'),
    }
    return js_conf
