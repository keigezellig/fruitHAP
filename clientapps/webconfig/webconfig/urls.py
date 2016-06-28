
"""webconfig URL Configuration

The `urlpatterns` list routes URLs to views. For more information please see:
    https://docs.djangoproject.com/en/1.9/topics/http/urls/
Examples:
Function views
    1. Add an import:  from my_app import views
    2. Add a URL to urlpatterns:  url(r'^$', views.home, name='home')
Class-based views
    1. Add an import:  from other_app.views import Home
    2. Add a URL to urlpatterns:  url(r'^$', Home.as_view(), name='home')
Including another URLconf
    1. Import the include() function: from django.conf.urls import url, include
    2. Add a URL to urlpatterns:  url(r'^blog/', include('blog.urls'))
"""
from django.conf.urls import url
from django.contrib import admin

from configurator import views
from configurator.views import SensorDetails
from configurator.views import SensorList

urlpatterns = [
    url(r'^$', views.index, name='index'),
    url(r'^configuration/sensors/(?P<sensor_name>[^/]+)', SensorDetails.as_view(), name='sensor_details'),
    url(r'^configuration/sensors/$', SensorList.as_view(), name='sensor_configuration'),
    url(r'^dashboard/$', views.dashboard, name='dashboard'),
    url(r'^admin/', admin.site.urls),
]
