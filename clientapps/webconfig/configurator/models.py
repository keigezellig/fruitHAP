from django.db import models
import urllib.parse


# Create your models here.
class Site(models.Model):
    name = models.CharField('Name', max_length=20, unique=True)
    description = models.TextField('Description', blank=True)
    hostname = models.CharField('Hostname', max_length=20)
    configPort = models.CharField('Configuration port', max_length=5, default='8888')
    mqPort = models.CharField('Message queue port', max_length=5, default='5672', blank=True)
    supervisordPort = models.CharField('Supervisord port', max_length=5, default='9001')

    def configuration_url(self):
        url_tuple = ('http', self.hostname, self.configPort, '', '', '')
        return urllib.parse.urlunparse(url_tuple)

    def supervisor_url(self):
        url_tuple = ('http', self.hostname, self.supervisordPort, '/RPC2', '', '')
        return urllib.parse.urlunparse(url_tuple)
